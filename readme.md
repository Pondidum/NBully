# NBully
*An implementation of the Bully algorithm*

## Resources
* https://en.wikipedia.org/wiki/Bully_algorithm
* https://en.wikipedia.org/wiki/Talk:Bully_algorithm
* http://www.cs.colostate.edu/~cs551/CourseNotes/Synchronization/BullyExample.html


* [x] on startup
  * [x] send `startElection` to all
* [ ] on `parentTimeout`
  * [ ] send `startElection` to all
* [x] on `startElection`
  * [x] if pid > source_pid
    * [x] send `alive` to source_pid
  * [x] if source_pid > pid
    * [x] add source_pid to known_parents
  * [x] begin `electionTimeout`
* [x] on `alive`
  * [x] if source_pid > pid
    * [x] add source_pid to known_parents
    * [ ] stop `electionTimeout`
  * [x] if pid > source_pid
    * [x] send `startElection` to all
* [x] on `electionTimeout`
  * [x] if no valid pids
    * [x] send `electionWon`
* [x] on `electionWon`
  * [x] stop `electionTimeout`
  * [x] if source_pid > pid
    * [x] send `startElection` to all
