# UnityMQ

Sample project to debug Unity + NetMQ see - https://github.com/zeromq/netmq/issues/631 'NetMQ + Unity3D, am I wasting my time?'

Project is:
* AsyncIO 0.1.26
* NetMQ 4.0.0-rc5
* UnityMQExamples\ReqResExample\RequestExample.cs
* UnityMQExamples\ReqResExample\ResponseExample.cs
* DebugScene.unity

To reproduce issue:
 1. Cloan: 
 2. Run (without MonoDebug connected) - Project will run OK
 3. Open Monodebug
 4. Start/Run Monodebug
 5. Return to Unity-> Run
 ... - Project runs for a few frames then freezes
 ... - Unity has to be forced quit.
