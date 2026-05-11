#include "std/mem.pat"

struct string {
    u32 len;
    char val[len];
};

struct fragment {
    u32 width;
    u32 height;
    u32 unknown[3];
    string file;
    float;
    u32;
    u32 x1;
    u32 y1;
    u32 x2;
    u32 y2;
    u32;
};

struct font {
    u32;
    string font;
    u32 fontchunk[10];
    u16;
};


struct testfrag {
    u32 width;
    u32 height;
    u32 unknown[3];
    string file;
    float;
};

struct object {
    u32 magic;
    if (magic == 1){
        if (minor == '3') {
            u8 m1[5];
            string strings[3];
        }
        else {
            u32;
        }
    }
    u32 x1;
    u32 y1;
    u32 x2;
    u32 y2;
    string name;
    u8 u_flags[3];
    string ini;
    u8 flags[4];
    u32 _x1; // matches earlier listed coordinates?
    u32 _y1; // ''
    u32 _x2; // ''
    u32 _y2; // ''
    u8;
    if (minor != '1') {
        testfrag;
    }
    else {
        u32 unk[2];
    }
    fragment tex[9]; // always 9 fragments
    u32 last;
    if (magic >= 3 && last != 0) {
        twofont = true;
    }
    if (magic == 2 && twofont) {
        twofont = false;
    }
    if (magic >= 3 && last == 0) { // have seen this as 4, if > 4 is encountered, change.
        font f;
        if (twofont && !std::mem::eof()){
            font;
        }
    }
};

struct int {
    char header[8];
    string filename;
    u8;
    u32;
    try {
        object o[while(!std::mem::eof())]; // just makes the pattern repeat until file end
    }
};

bool twofont = false;
char minor @ 0x7;
int file @ 0x0;
