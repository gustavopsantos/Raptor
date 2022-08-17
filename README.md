<div align=center><img src="Graphics/raptor.png" width="256" height="256"></div>

<div align=center>   

# Raptor
### Modern async .NET transport layer <a href="https://en.wikipedia.org/wiki/Proof_of_concept">PoC</a>
Raptor is an extension on top of the User Datagram Protocol (<a href="https://en.wikipedia.org/wiki/User_Datagram_Protocol">UDP</a>), adding a layer of reliability (<a href="https://en.wikipedia.org/wiki/Automatic_repeat_request">ARQ</a>) in the delivery of packets when required.

⚠️ Since this is still a PoC I dont recomment using it in any project. ⚠️
</div>

## Simulation
Raptor does not comes with simulation tool, since there are already good tools for that such as <a href="https://github.com/jagt/clumsy">Clumsy</a>, I see no point of adding it inside the protocol.

## Transport Layer Roadmap
- [ ] 🔒 Encryption
- [x] ♻️ ARQ Protocol
- [ ] 🛡️ Attacks Protection
- [x] ✒️ Serialization Protocol
- [ ] 🔗 Connection Maintainer (Keep-Alive)

## Sample Game Roadmap
- [x] 🔁 RTT Measurement
- [x] 🎮 Input Buffering
- [ ] ⌛ Server Rollback
- [ ] 🔮 Client Prediction
- [ ] 🎚️ Upstream Throttle
- [ ] 🚨 Server Reconciliation
- [x] ⌚ Time Synchonization
- [x] ⏱️ Timing Synchonization
- [ ] 👻 Semi Fixed Timestep Interpolation

<div align=center>

## Credits
<a href="https://www.flaticon.com/free-icons/dino" title="dino icon">Raptor icon created by max.icons - Flaticon</a>
</div>
