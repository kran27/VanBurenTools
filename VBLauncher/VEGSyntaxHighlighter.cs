using System;
using ImGuiColorTextEditNet;

namespace VBLauncher;

public class VEGSyntaxHighlighter : ISyntaxHighlighter
{
    private static readonly object DefaultState = new();
    private static readonly object MultiLineCommentState = new();
    private readonly SimpleTrie<Identifier> _identifiers;

    private record Identifier(PaletteIndex Color)
    {
        public string Declaration = "";
    }

    public VEGSyntaxHighlighter()
    {
        var language = VEGLang();

        _identifiers = new SimpleTrie<Identifier>();
        if (language.Keywords != null)
            foreach (var keyword in language.Keywords)
                _identifiers.Add(keyword, new Identifier(PaletteIndex.Keyword));

        if (language.Identifiers != null)
        {
            foreach (var name in language.Identifiers)
            {
                var identifier = new Identifier(PaletteIndex.KnownIdentifier)
                {
                    Declaration = "Enumeration"
                };
                _identifiers.Add(name, identifier);
            }
        }

        foreach (var str in VEG.AllEnumValues()) _identifiers.Add(str, new Identifier(PaletteIndex.Custom));

        string[] heads = ["VFX", "VEG"];
        foreach (var str in heads) _identifiers.Add(str, new Identifier(PaletteIndex.Custom + 1));

        string[] funcs = ["b3d", "skl", "gr2", "tga", "psf", "wav"];
        foreach (var str in funcs) _identifiers.Add(str, new Identifier(PaletteIndex.Custom + 2));
    }

    public bool AutoIndentation => true;
    public int MaxLinesPerFrame => 1000;
    public string? GetTooltip(string id)
    {
        var info = _identifiers.Get(id);
        return info?.Declaration;
    }

    public object Colorize(Span<Glyph> line, object? state)
    {
        for (var i = 0; i < line.Length;)
        {
            var result = Tokenize(line[i..], ref state);
            Util.Assert(result != 0);

            if (result == -1)
            {
                line[i] = new Glyph(line[i].Char, PaletteIndex.Default);
                i++;
            }
            else i += result;
        }

        return state ?? DefaultState;
    }

    private int Tokenize(Span<Glyph> span, ref object? state)
    {
        var i = 0;

        // Skip leading whitespace
        while (i < span.Length && span[i].Char is ' ' or '\t')
            i++;

        if (i > 0)
            return i;

        int result;
        if ((result = TokenizeMultiLineComment(span, ref state)) != -1) return result;
        if ((result = TokenizeSingleLineComment(span)) != -1) return result;
        if ((result = TokenizePreprocessorDirective(span)) != -1) return result;
        if ((result = TokenizeCStyleString(span)) != -1) return result;
        if ((result = TokenizeCStyleCharacterLiteral(span)) != -1) return result;
        if ((result = TokenizeCStyleIdentifier(span)) != -1) return result;
        if ((result = TokenizeCStyleNumber(span)) != -1) return result;
        if ((result = TokenizeCStylePunctuation(span)) != -1) return result;
        return -1;
    }

    private static int TokenizeMultiLineComment(Span<Glyph> span, ref object? state)
    {
        var i = 0;
        if (state != MultiLineCommentState && (span[i].Char != '/' || 1 >= span.Length || span[1].Char != '*'))
            return -1;

        state = MultiLineCommentState;
        for (; i < span.Length; i++)
        {
            span[i] = new Glyph(span[i].Char, PaletteIndex.MultiLineComment);
            if (span[i].Char != '*' || i + 1 >= span.Length || span[i + 1].Char != '/') continue;
            i++;
            span[i] = new Glyph(span[i].Char, PaletteIndex.MultiLineComment);
            state = DefaultState;
            return i;
        }

        return i;
    }

    private static int TokenizeSingleLineComment(Span<Glyph> span)
    {
        if (span[0].Char != '/' || 1 >= span.Length || span[1].Char != '/')
            return -1;

        for (var i = 0; i < span.Length; i++)
            span[i] = new Glyph(span[i].Char, PaletteIndex.Comment);

        return span.Length;
    }

    private static int TokenizePreprocessorDirective(Span<Glyph> span)
    {
        if (span[0].Char != '#')
            return -1;

        for (var i = 0; i < span.Length; i++)
            span[i] = new Glyph(span[i].Char, PaletteIndex.Preprocessor);

        return span.Length;
    }

    private static LanguageDefinition VEGLang() => new("VEG")
    {
        Keywords =
        [
            "int", "VFX_Byte", "float", "bool", "enum", "std", "string", "VFX_Vector", "VFX_Vector2", "VFX_Rotation",
            "VFX_Color", "VFX_Resource", "VFX_TextureUVs", "VFX_SnapBone", "VFX_TriggeredEffect",
            "none"
        ],
        Identifiers = VEG.enums
    };

    private static int TokenizeCStyleString(Span<Glyph> input)
    {
        if (input[0].Char != '"')
            return -1;
        for (var i = 1; i < input.Length; i++)
        {
            var c = input[i].Char;

            if (c != '"' || input[i - 1].Char == '\\') continue;

            for (var j = 0; j <= i; j++)
            {
                input[j] = new Glyph(input[j].Char, PaletteIndex.String);
            }

            return i + 1;
        }
        return -1;
    }

    private static int TokenizeCStyleCharacterLiteral(Span<Glyph> input)
    {
        var i = 0;

        if (input[i++].Char != '\'')
            return -1;

        if (i < input.Length && input[i].Char == '\\')
            i++; // handle escape characters

        i++; // Skip actual char

        // handle end of character literal
        if (i >= input.Length || input[i].Char != '\'')
            return -1;

        for (var j = 0; j < i; j++)
            input[j] = new Glyph(input[j].Char, PaletteIndex.CharLiteral);

        return i;
    }

    private int TokenizeCStyleIdentifier(Span<Glyph> input)
    {
        var i = 0;

        var c = input[i].Char;
        if (!char.IsLetter(c) && c != '_')
            return -1;

        i++;

        for (; i < input.Length; i++)
        {
            c = input[i].Char;
            if (c != '_' && !char.IsLetterOrDigit(c))
                break;
        }

        var info = _identifiers.Get<Glyph>(input[..i], x => x.Char);

        for (var j = 0; j < i; j++)
            input[j] = new Glyph(input[j].Char, info?.Color ?? PaletteIndex.Identifier);

        return i;
    }

    private static int TokenizeCStyleNumber(Span<Glyph> input)
    {
        var i = 0;
        var c = input[i].Char;

        var startsWithNumber = char.IsNumber(c);

        if (c != '+' && c != '-' && !startsWithNumber)
            return -1;

        i++;

        var hasNumber = startsWithNumber;
        while (i < input.Length && char.IsNumber(input[i].Char))
        {
            hasNumber = true;
            i++;
        }

        if (!hasNumber)
            return -1;

        var isFloat = false;
        var isHex = false;
        var isBinary = false;

        if (i < input.Length)
        {
            if (input[i].Char == '.')
            {
                isFloat = true;

                i++;
                while (i < input.Length && char.IsNumber(input[i].Char))
                    i++;
            }
            else if (input[i].Char is 'x' or 'X' && i == 1 && input[i].Char == '0')
            {
                // hex formatted integer of the type 0xef80
                isHex = true;

                i++;
                for (; i < input.Length; i++)
                {
                    c = input[i].Char;
                    if (!char.IsNumber(c) && c is not (>= 'a' and <= 'f') && c is not (>= 'A' and <= 'F'))
                        break;
                }
            }
            else if (input[i].Char is 'b' or 'B' && i == 1 && input[i].Char == '0')
            {
                // binary formatted integer of the type 0b01011101

                isBinary = true;

                i++;
                for (; i < input.Length; i++)
                {
                    c = input[i].Char;
                    if (c != '0' && c != '1')
                        break;
                }
            }
        }

        if (!isHex && !isBinary)
        {
            // floating point exponent
            if (i < input.Length && input[i].Char is 'e' or 'E')
            {
                isFloat = true;

                i++;

                if (i < input.Length && input[i].Char is '+' or '-')
                    i++;

                var hasDigits = false;
                while (i < input.Length && input[i].Char is >= '0' and <= '9')
                {
                    hasDigits = true;
                    i++;
                }

                if (!hasDigits)
                    return -1;
            }

            // single precision floating point type
            if (i < input.Length && input[i].Char == 'f')
                i++;
        }

        if (!isFloat)
        {
            // integer size type
            while (i < input.Length && input[i].Char is 'u' or 'U' or 'l' or 'L')
                i++;
        }

        for (var j = 0; j < i; j++)
            input[j] = new Glyph(input[j].Char, PaletteIndex.Number);

        return i;
    }

    private static int TokenizeCStylePunctuation(Span<Glyph> input)
    {
        switch (input[0].Char)
        {
            case '[':
            case ']':
            case '{':
            case '}':
            case '(':
            case ')':
            case '-':
            case '+':
            case '<':
            case '>':
            case '?':
            case ':':
            case ';':
            case '!':
            case '%':
            case '^':
            case '&':
            case '|':
            case '*':
            case '/':
            case '=':
            case '~':
            case ',':
            case '.':
                input[0] = new Glyph(input[0].Char, PaletteIndex.Punctuation);
                return 1;
            default:
                return -1;
        }
    }
}