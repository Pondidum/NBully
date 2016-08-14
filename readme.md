
* [ ] on startup or `parentTimeout`
  * [ ] send `startElection` to all


* [ ] on `startElection`
  * [ ] if pid > source_pid
    * [ ] send `alive` to source_pid
  * [x] begin `electionTimeout`


* [ ] on `alive`
  * [ ] if source_pid > pid
    * [ ] stop  //possibly re-send election if nothing happens
  * [ ] if pid > source_pid
    * [ ] send `startElection` to all


* [x] on `electionTimeout`
  * [x] if no valid pids
    * [x] send `electionWon`


* [ ] on `electionWon`
  * [ ] stop `electionTimeout`
