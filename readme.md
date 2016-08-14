
* [ ] on startup or `parentTimeout`
  * [ ] send `startElection` to all


* [x] on `startElection`
  * [x] if pid > source_pid
    * [x] send `alive` to source_pid
  * [x] if source_pid > pid
    * [x] add source_pid to known_parents
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
