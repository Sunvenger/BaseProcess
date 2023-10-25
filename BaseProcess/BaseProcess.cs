using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Threading;
namespace BigDataProcessor.Processes
{
	public class BaseProcess
	{
		string ProcessName;
		public Queue<String> qStatus;
		public Queue<String> qControl;
		public List<String> StatusLog;
		public string LastStatus;
		private StreamWriter LogWriter;
		public bool isEnabledLog = false;
		public string PathToLog { get; set; } = "";
		public delegate void MainProcessSpaceDelegate ();
		public MainProcessSpaceDelegate MainProcessSpace { get; set; }
		public delegate void OnStatusReceivedDelegate(string status);
		public OnStatusReceivedDelegate OnStatusReceived{ get; set; }

		public Task Runner;

		public BaseProcess()
		{
			string ProcessName = this.GetType().Name;
			qStatus = new Queue<String>();
			qControl = new Queue<String>();
			Runner = new Task(() => MainProcessSpace());

	    }
		public bool HaveNewStatus()
		{
			if (qStatus.Count > 0) return true;
			else return false;
		}

		public string GenerateLogPath()
		{
			return $"log/{this.GetType().Name}.txt";
		}
		public void EnableLogging(string path = "")
		{
			if (path == "")
			{
				if (PathToLog == "") // No path to log
					PathToLog = GenerateLogPath();
			}
			else
				PathToLog = path;
			isEnabledLog = true;

			LogWriter = new StreamWriter(PathToLog);
		}

		public void DisableLogging()
		{
			if(LogWriter!=null)
			LogWriter.Close();
			isEnabledLog = false;
		}

		public void AppendLog(string messageLog)
		{
			if (isEnabledLog)
				LogWriter.WriteLine($"{DateTime.Now} : {messageLog}");
			else
			{
				
			}
		}

		public void StartProcess()
		{
			Runner.Start();
		}

		public void FinishProcess()
		{
			AppendLog($"'{ProcessName}' Proccess finished");
			DisableLogging();
		}

	public void SendStatus(string status)
		{
			OnStatusReceived(status);	
			qStatus.Enqueue(status);
			AppendLog($"Status changed: {status}");
			LastStatus = status;
		}

		public string ReceiveStatus()
		{
			if (qStatus.Count > 0)
				return (qStatus.Dequeue());
			else
				return null;
		}

		public void SendCommand(string command)
		{
			qControl.Enqueue(command);
		}

		public bool IncomingCommandExist()
		{
			if (qControl.Count > 0)
			{
				return true;
			}
			else
				return false;
		}


		public string ReceiveCommand()
		{
			if (qControl.Count > 0)
			{
				return (qControl.Dequeue());
			}
			else return null;
		}
		public string ReceiveControl()
		{
			if (qControl.Count > 0)
				return (qControl.Dequeue());
			else
				return null;
		}
	}


}
