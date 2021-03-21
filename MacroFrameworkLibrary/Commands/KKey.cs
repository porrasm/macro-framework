namespace MacroFramework.Commands {
    /// <summary>An enhanced list of Virtual Keys.</summary>
    public enum KKey : uint {
        /// <summary>Flag for custom defined keys</summary>
        F_Custom = 1 << 8,
        /// <summary>Flag for modifier keys</summary>
        F_Modifier = F_Custom << 1,
        /// <summary>Flag for mouse keys</summary>
        F_Mouse = F_Custom << 2,
        /// <summary>Flag for numpad keys</summary>
        F_Numpad = F_Custom << 3,
        /// <summary>Flag for scroll keys</summary>
        F_Scroll = F_Custom << 4,
        /// <summary>Flag for number keys</summary>
        F_Number = F_Custom << 5,
        /// <summary>Flag for extended keys</summary>
        F_Extended = F_Custom << 6,
        /// <summary>Flag for media keys</summary>
        F_Media = F_Custom << 7,
        /// <summary>Flag for keys that produce visible characters</summary>
        F_Char = F_Custom << 8,
        /// <summary>Flag for stateless keys</summary>
        F_Stateless = F_Custom << 10,
        /// <summary>Flag for toggle keys</summary>
        F_Toggle = F_Custom << 11,

        /// <summary>Flag for shift keys</summary>
        Shift = F_Custom << 20 | F_Modifier,
        /// <summary>Flag for ctrl keys</summary>
        Ctrl = F_Custom << 21 | F_Modifier,
        /// <summary>Flag for win keys</summary>
        Win = F_Custom << 22 | F_Modifier,
        /// <summary>Flag for alt keys</summary>
        Alt = F_Custom << 23 | F_Modifier,

        /// <summary>Mask for the flag bits</summary>
        M_FlagMask = 0xFFFFFF00,
        /// <summary>Mask for the Virtual Key</summary>
        M_KeyMask = 0xFF,

        // -------------------------- //

        /// <summary>Key is undefined</summary>
        Undefined = VKey.UNDEFINED,

        /// <summary>Left mouse button</summary>
        MouseLeft = VKey.LBUTTON | F_Mouse,
        /// <summary>Right mouse button</summary>
        MouseRight = VKey.RBUTTON | F_Mouse,
        /// <summary>Middle mouse button</summary>
        MouseMiddle = VKey.MBUTTON | F_Mouse,
        /// <summary>Extra mouse button 1</summary>
        MouseXButton1 = VKey.XBUTTON1 | F_Mouse,
        /// <summary>Extra mouse button 2</summary>
        MouseXButton2 = VKey.XBUTTON2 | F_Mouse,

        /// <summary>Digit 0</summary>
        D0 = VKey.D0 | F_Number | F_Char,
        /// <summary>Digit 1</summary>
        D1 = VKey.D1 | F_Number | F_Char,
        /// <summary>Digit 2</summary>
        D2 = VKey.D2 | F_Number | F_Char,
        /// <summary>Digit 3</summary>
        D3 = VKey.D3 | F_Number | F_Char,
        /// <summary>Digit 4</summary>
        D4 = VKey.D4 | F_Number | F_Char,
        /// <summary>Digit 5</summary>
        D5 = VKey.D5 | F_Number | F_Char,
        /// <summary>Digit 6</summary>
        D6 = VKey.D6 | F_Number | F_Char,
        /// <summary>Digit 7</summary>
        D7 = VKey.D7 | F_Number | F_Char,
        /// <summary>Digit 8</summary>
        D8 = VKey.D8 | F_Number | F_Char,
        /// <summary>Digit 9</summary>
        D9 = VKey.D9 | F_Number | F_Char,

        A = VKey.A | F_Char,
        B = VKey.B | F_Char,
        C = VKey.C | F_Char,
        D = VKey.D | F_Char,
        E = VKey.E | F_Char,
        F = VKey.F | F_Char,
        G = VKey.G | F_Char,
        H = VKey.H | F_Char,
        I = VKey.I | F_Char,
        J = VKey.J | F_Char,
        K = VKey.K | F_Char,
        L = VKey.L | F_Char,
        M = VKey.M | F_Char,
        N = VKey.N | F_Char,
        O = VKey.O | F_Char,
        P = VKey.P | F_Char,
        Q = VKey.Q | F_Char,
        R = VKey.R | F_Char,
        S = VKey.S | F_Char,
        T = VKey.T | F_Char,
        U = VKey.U | F_Char,
        V = VKey.V | F_Char,
        W = VKey.W | F_Char,
        X = VKey.X | F_Char,
        Y = VKey.Y | F_Char,
        Z = VKey.Z | F_Char,

        Numpad0 = VKey.NUMPAD0 | F_Numpad | F_Number | F_Char,
        Numpad1 = VKey.NUMPAD1 | F_Numpad | F_Number | F_Char,
        Numpad2 = VKey.NUMPAD2 | F_Numpad | F_Number | F_Char,
        Numpad3 = VKey.NUMPAD3 | F_Numpad | F_Number | F_Char,
        Numpad4 = VKey.NUMPAD4 | F_Numpad | F_Number | F_Char,
        Numpad5 = VKey.NUMPAD5 | F_Numpad | F_Number | F_Char,
        Numpad6 = VKey.NUMPAD6 | F_Numpad | F_Number | F_Char,
        Numpad7 = VKey.NUMPAD7 | F_Numpad | F_Number | F_Char,
        Numpad8 = VKey.NUMPAD8 | F_Numpad | F_Number | F_Char,
        Numpad9 = VKey.NUMPAD9 | F_Numpad | F_Number | F_Char,
        NumpadDot = VKey.DECIMAL | F_Numpad | F_Char,

        NumpadIns = VKey.INSERT | F_Numpad,
        NumpadEnd = VKey.END | F_Numpad,
        NumpadDown = VKey.DOWN | F_Numpad,
        NumpadPgDn = VKey.NEXT | F_Numpad,
        NumpadLeft = VKey.LEFT | F_Numpad,
        NumpadClear = VKey.CLEAR | F_Numpad,
        NumpadRight = VKey.RIGHT | F_Numpad,
        NumpadHome = VKey.HOME | F_Numpad,
        NumpadUp = VKey.UP | F_Numpad,
        NumpadPgUp = VKey.PRIOR | F_Numpad,
        NumpadDel = VKey.DELETE | F_Numpad,

        NumpadDiv = VKey.DIVIDE | F_Numpad | F_Char | F_Extended,
        NumpadMult = VKey.MULTIPLY | F_Numpad | F_Char,
        NumpadSub = VKey.SUBTRACT | F_Numpad | F_Char,
        NumpadAdd = VKey.ADD | F_Numpad | F_Char,
        NumpadEnter = VKey.RETURN | F_Numpad | F_Extended,

        Numlock = VKey.NUMLOCK | F_Numpad | F_Extended | F_Toggle,
        ScrollLock = VKey.SCROLL | F_Toggle,
        CapsLock = VKey.CAPITAL | F_Toggle,

        F1 = VKey.F1,
        F2 = VKey.F2,
        F3 = VKey.F3,
        F4 = VKey.F4,
        F5 = VKey.F5,
        F6 = VKey.F6,
        F7 = VKey.F7,
        F8 = VKey.F8,
        F9 = VKey.F9,
        F10 = VKey.F10,
        F11 = VKey.F11,
        F12 = VKey.F12,

        F13 = VKey.F13,
        F14 = VKey.F14,
        F15 = VKey.F15,
        F16 = VKey.F16,
        F17 = VKey.F17,
        F18 = VKey.F18,
        F19 = VKey.F19,
        F20 = VKey.F20,
        F21 = VKey.F21,
        F22 = VKey.F22,
        F23 = VKey.F23,
        F24 = VKey.F24,

        /// <summary>Left arrow key</summary>
        Left = VKey.LEFT | F_Extended,
        /// <summary>Right arrow key</summary>
        Right = VKey.RIGHT | F_Extended,
        /// <summary>Up arrow key</summary>
        Up = VKey.UP | F_Extended,
        /// <summary>Down arrow key</summary>
        Down = VKey.DOWN | F_Extended,

        /// <summary>Left shift key</summary>
        LShift = VKey.LSHIFT | Shift,
        /// <summary>Right shift key</summary>
        RShift = VKey.RSHIFT | Shift,
        /// <summary>Left control key</summary>
        LCtrl = VKey.LCONTROL | Ctrl,
        /// <summary>Right control key</summary>
        RCtrl = VKey.RCONTROL | Ctrl | F_Extended,
        /// <summary>Left alt key</summary>
        LAlt = VKey.LMENU | Alt,
        /// <summary>Right alt key</summary>
        RAlt = VKey.RMENU | Alt | F_Extended,
        /// <summary>Left win key</summary>
        LWin = VKey.LWIN | Win,
        /// <summary>Right win key</summary>
        RWin = VKey.RWIN | Win,

        Enter = VKey.RETURN | F_Char,
        Space = VKey.SPACE | F_Char,
        Tab = VKey.TAB | F_Char,
        Backspace = VKey.BACK,
        Escape = VKey.ESCAPE,
        /// <summary>Context menu key</summary>
        App = VKey.APPS,
        /// <summary>Context menu key</summary>
        Context = App,

        /// <summary>Print screen key</summary>
        PrintScrn = VKey.SNAPSHOT | F_Extended,
        Pause = VKey.PAUSE,
        Insert = VKey.INSERT | F_Extended,
        Home = VKey.HOME | F_Extended,
        End = VKey.END | F_Extended,
        Delete = VKey.DELETE | F_Extended,
        PageUp = VKey.PRIOR | F_Extended,
        PageDown = VKey.NEXT | F_Extended,

        /// <summary>The general back key</summary>
        BrowserBack = VKey.BROWSER_BACK,
        /// <summary>The general forward key</summary>
        BrowserForward = VKey.BROWSER_FORWARD,
        BrowserFavorites = VKey.BROWSER_FAVORITES,
        BrowserHome = VKey.BROWSER_HOME,
        BrowserRefresh = VKey.BROWSER_REFRESH,
        BrowserSearch = VKey.BROWSER_SEARCH,
        BrowserStop = VKey.BROWSER_STOP,

        MediaNext = VKey.MEDIA_NEXT_TRACK | F_Media,
        MediaPrev = VKey.MEDIA_PREV_TRACK | F_Media,
        MediaStop = VKey.MEDIA_STOP | F_Media,
        /// <summary>Media play pause key</summary>
        MediaPlay = VKey.MEDIA_PLAY_PAUSE | F_Media,

        VolumeUp = VKey.VOLUME_UP | F_Media,
        VolumeDown = VKey.VOLUME_DOWN | F_Media,
        VolumeMute = VKey.VOLUME_MUTE | F_Media,

        LaunchApp1 = VKey.LAUNCH_APP1,
        LaunchApp2 = VKey.LAUNCH_APP2,
        LaunchMail = VKey.LAUNCH_MAIL,
        LaunchMediaSelect = VKey.LAUNCH_MEDIA_SELECT,

        Plus = VKey.OEM_PLUS | F_Char,
        Minus = VKey.OEM_MINUS | F_Char,
        Comma = VKey.OEM_COMMA | F_Char,
        Period = VKey.OEM_PERIOD | F_Char,

        /// <summary>The ¨ key</summary>
        Umlaut = VKey.OEM_1 | F_Char,
        /// <summary>The ' key</summary>
        Apostrophe = VKey.OEM_2 | F_Char,
        /// <summary>The Ö key</summary>
        Ö = VKey.OEM_3 | F_Char,
        /// <summary>The ´ key</summary>
        tilde = VKey.OEM_4 | F_Char,
        /// <summary>The § key</summary>
        Section = VKey.OEM_5 | F_Char,
        /// <summary>The Å key</summary>
        Å = VKey.OEM_6 | F_Char,
        /// <summary>The Ä key</summary>
        Ä = VKey.OEM_7 | F_Char,
        /// <summary>The &lt; key</summary>
        Less = VKey.OEM_102 | F_Char,

        OEM_1 = Umlaut,
        OEM_2 = Apostrophe,
        OEM_3 = Ö,
        OEM_4 = tilde,
        OEM_5 = Section,
        OEM_6 = Å,
        OEM_7 = Ä,
        OEM_8 = VKey.OEM_8,
        OEM_102 = Less,
        OEM_Clear = VKey.OEM_CLEAR,
        OEM_Plus = Plus,
        OEM_Minus = Minus,
        OEM_Comma = Comma,
        OEM_Period = Period,

        // Other //

        Packet = VKey.PACKET,
        Kana = VKey.KANA,
        Junja = VKey.JUNJA,
        Kanji = VKey.KANJI,
        Convert = VKey.CONVERT,
        NonConvert = VKey.NONCONVERT,
        ModeChange = VKey.MODECHANGE,
        Break = VKey.CANCEL | F_Extended,
        Clear = VKey.CLEAR | F_Extended,
        Print = VKey.PRINT,
        Final = VKey.FINAL,
        Accept = VKey.ACCEPT,
        Select = VKey.SELECT,
        Execute = VKey.EXECUTE,
        Help = VKey.HELP,
        Sleep = VKey.SLEEP,
        Separator = VKey.SEPARATOR,
        ProcessKey = VKey.PROCESSKEY,
        Attn = VKey.ATTN,
        CrSel = VKey.CRSEL,
        ExSel = VKey.EXSEL,
        EraseEOF = VKey.EREOF,
        Play = VKey.PLAY,
        Zoom = VKey.ZOOM,
        NoName = VKey.NONAME,
        Pa1 = VKey.PA1,

        // Custom //

        /// <summary>Mouse wheel left</summary>
        WheelLeft = 1 | F_Scroll | F_Mouse | F_Custom | F_Stateless,
        /// <summary>Mouse wheel right</summary>
        WheelRight = 2 | F_Scroll | F_Mouse | F_Custom | F_Stateless,
        /// <summary>Mouse wheel up</summary>
        WheelUp = 3 | F_Scroll | F_Mouse | F_Custom | F_Stateless,
        /// <summary>Mouse wheel down</summary>
        WheelDown = 4 | F_Scroll | F_Mouse | F_Custom | F_Stateless,

        /// <summary>Mouse movement</summary>
        MouseMove = 5 | F_Mouse | F_Custom | F_Stateless,

        /// <summary>
        /// The <see cref="MacroSettings.GeneralBindKey"/> key will always be transformed to this key
        /// </summary>
        GeneralBindKey = 6 | F_Custom,

        /// <summary>
        /// Does not represent any key
        /// </summary>
        None = 7 | F_Custom
    }
}
