namespace PortMidi
{
    // Filter bit-mask definitions
    public enum PortMidiFilter
    {
        // filter active sensing messages (0xFE)
        PM_FILT_ACTIVE = (1 << 0x0E),
        // filter system exclusive messages (0xF0)
        PM_FILT_SYSEX = (1 << 0x00),
        // filter MIDI clock message (0xF8)
        PM_FILT_CLOCK = (1 << 0x08),
        // filter play messages (start 0xFA, stop 0xFC, continue 0xFB)
        PM_FILT_PLAY = ((1 << 0x0A) | (1 << 0x0C) | (1 << 0x0B)),
        // filter tick messages (0xF9)
        PM_FILT_TICK = (1 << 0x09),
        // filter undefined FD messages
        PM_FILT_FD = (1 << 0x0D),
        // filter undefined real-time messages
        PM_FILT_UNDEFINED = PM_FILT_FD,
        // filter reset messages (0xFF)
        PM_FILT_RESET = (1 << 0x0F),
        // filter all real-time messages
        PM_FILT_REALTIME = (PM_FILT_ACTIVE | PM_FILT_SYSEX | PM_FILT_CLOCK | PM_FILT_PLAY | PM_FILT_UNDEFINED | PM_FILT_RESET | PM_FILT_TICK),
        // filter note-on and note-off (0x90-0x9F and 0x80-0x8F
        PM_FILT_NOTE = ((1 << 0x19) | (1 << 0x18)),
        // filter channel aftertouch (most midi controllers use this) (0xD0-0xDF)
        PM_FILT_CHANNEL_AFTERTOUCH = (1 << 0x1D),
        // per-note aftertouch (0xA0-0xAF)
        PM_FILT_POLY_AFTERTOUCH = (1 << 0x1A),
        // filter both channel and poly aftertouch
        PM_FILT_AFTERTOUCH = (PM_FILT_CHANNEL_AFTERTOUCH | PM_FILT_POLY_AFTERTOUCH),
        // Program changes (0xC0-0xCF)
        PM_FILT_PROGRAM = (1 << 0x1C),
        // Control Changes (CC's) (0xB0-0xBF)
        PM_FILT_CONTROL = (1 << 0x1B),
        // Pitch Bender (0xE0-0xEF)
        PM_FILT_PITCHBEND = (1 << 0x1E),
        // MIDI Time Code (0xF1)
        PM_FILT_MTC = (1 << 0x01),
        // Song Position (0xF2)
        PM_FILT_SONG_POSITION = (1 << 0x02),
        // Song Select (0xF3)
        PM_FILT_SONG_SELECT = (1 << 0x03),
        // Tuning request (0xF6)
        PM_FILT_TUNE = (1 << 0x06),
        // All System Common messages (mtc, song position, song select, tune request)
        PM_FILT_SYSTEMCOMMON = (PM_FILT_MTC | PM_FILT_SONG_POSITION | PM_FILT_SONG_SELECT | PM_FILT_TUNE),
    }
}
