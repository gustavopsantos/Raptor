<div align=center>   

# Raptor
### Modern async .NET transport layer <a href="https://en.wikipedia.org/wiki/Proof_of_concept">PoC</a>
Raptor is an extension on top of the User Datagram Protocol (<a href="https://en.wikipedia.org/wiki/User_Datagram_Protocol">UDP</a>), adding a layer of reliability (<a href="https://en.wikipedia.org/wiki/Automatic_repeat_request">ARQ</a>) in the delivery of packets when required.

⚠️ Since this is still a PoC I dont recomment using it in any project. ⚠️
</div>

# Simulation
Raptor does not comes with simulation tool, since there are already good tools for that such as <a href="https://github.com/jagt/clumsy">Clumsy</a>, I see no point of adding it inside the protocol.

# TODO  
- Adds connection maintainer (Keep-Alive)  
- Adds sample game  
- Improves ARQ protocol  
- Adds unit tests  
- Removes unity logger dependency from Raptor assembly, and make it pure .NET   
- Improves serialization performance and memory footprint  
- Improves code readability  
- Adds security  
- Adds RTT measurements/benchmark for all APIs
