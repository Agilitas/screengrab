namespace ScreenGrab.ClickOnce {
    public class UpdateCompleteEventArgs {
        public bool NeedsRestart { get; private set; }
        public UpdateCompleteEventArgs(bool needsRestart) {
            NeedsRestart = needsRestart;
        }
    }
}