using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayer
{
    class Constant
    {
        public const string PlaylistPath = "playlist.xml";
        public const string CurrentPlaylist  = "currentplaylist.xml";
        // track
        public const string CurrentTrack = "trackname";
        public const string StartPlayback = "startplayback";
        public const string SkipNext = "skipnext";
        public const string Position = "position";
        public const string SkipPrevious = "skipprevious";
        public const string Trackchanged = "songchanged";

        // background 
        public const string BackgroundTaskStarted = "BackgroundTaskStarted";
        public const string BackgroundTaskRunning = "BackgroundTaskRunning";
        public const string BackgroundTaskCancelled = "BackgroundTaskCancelled";
        public const string BackgroundTaskState = "backgroundtaskstate";

        // foreground
        public const string ForegroundAppActive = "Active";
        public const string ForegroundAppSuspended = "Suspended";

        // app
        public const string AppSuspended = "appsuspend";
        public const string AppResumed = "appresumed";
        public const string AppState = "appstate";

    }
    class Command
    {

        public const string InitList = "initlist";

        public const string SetCurrentIndex = "setcurrentindex";

        public const string PlayWithIndex = "playwithindex";

        /// <summary>
        /// hiểu là resume nhé 
        /// </summary>
        public const string Play = "play"; 

        public const string Pause = "pause";

        public const string Shuffle = "shuffle";

        public const string Next = "next";

        public const string Previous = "previous";

        public const string LoopState = "loopstate";

        public const string Titte = "title";

    }

    /*
     * ở fore ground chỉ cần kiểm tra trạng thái của nút bấm loop
     * rồi gởi message đến background với lệnh Command.LoopState (ở trên)
     * đối số thứ 2 là một trong 3 trạng thái sau.
     */
    enum eLoopState
    {
        None,
        One,
        All,
    }

    /*
     * muốn set trạng thái nút pause / resume thì 
     * gởi message đến background với lệnh Command.Pause hoặc Command.Play (ở trên)
     */
    
}
