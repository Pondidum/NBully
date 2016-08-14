
* on startup or `parentTimeout`
  * send `startElection` to all


* on `startElection`
  * if pid > source_pid
    * send `alive` to source_pid
  * begin `electionTimeout`


* on `alive`
  * if source_pid > pid
    * stop  //possibly re-send election if nothing happens
  * if pid > source_pid
    * send `startElection` to all


* on `electionTimeout`
  * if no valid pids
    * send `electionWon`


* on `electionWon`
  * stop `electionTimeout`
