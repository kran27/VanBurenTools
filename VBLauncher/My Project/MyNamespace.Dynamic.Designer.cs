using System;
using System.ComponentModel;
using System.Diagnostics;

namespace VBLauncher.My
{
    internal static partial class MyProject
    {
        internal partial class MyForms
        {

            [EditorBrowsable(EditorBrowsableState.Never)]
            public Editor m_Editor;

            public Editor Editor
            {
                [DebuggerHidden]
                get
                {
                    m_Editor = Create__Instance__(m_Editor);
                    return m_Editor;
                }
                [DebuggerHidden]
                set
                {
                    if (ReferenceEquals(value, m_Editor))
                        return;
                    if (value is not null)
                        throw new ArgumentException("Property can only be set to Nothing");
                    Dispose__Instance__(ref m_Editor);
                }
            }


            [EditorBrowsable(EditorBrowsableState.Never)]
            public Main m_Main;

            public Main Main
            {
                [DebuggerHidden]
                get
                {
                    m_Main = Create__Instance__(m_Main);
                    return m_Main;
                }
                [DebuggerHidden]
                set
                {
                    if (ReferenceEquals(value, m_Main))
                        return;
                    if (value is not null)
                        throw new ArgumentException("Property can only be set to Nothing");
                    Dispose__Instance__(ref m_Main);
                }
            }


            [EditorBrowsable(EditorBrowsableState.Never)]
            public ModLoader m_ModLoader;

            public ModLoader ModLoader
            {
                [DebuggerHidden]
                get
                {
                    m_ModLoader = Create__Instance__(m_ModLoader);
                    return m_ModLoader;
                }
                [DebuggerHidden]
                set
                {
                    if (ReferenceEquals(value, m_ModLoader))
                        return;
                    if (value is not null)
                        throw new ArgumentException("Property can only be set to Nothing");
                    Dispose__Instance__(ref m_ModLoader);
                }
            }


            [EditorBrowsable(EditorBrowsableState.Never)]
            public Options m_Options;

            public Options Options
            {
                [DebuggerHidden]
                get
                {
                    m_Options = Create__Instance__(m_Options);
                    return m_Options;
                }
                [DebuggerHidden]
                set
                {
                    if (ReferenceEquals(value, m_Options))
                        return;
                    if (value is not null)
                        throw new ArgumentException("Property can only be set to Nothing");
                    Dispose__Instance__(ref m_Options);
                }
            }

        }


    }
}