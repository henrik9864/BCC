using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BCC.Commands
{
	class RunMethod : ICommand
	{
		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		Action<object> method;
		Func<object, bool> canRun;

		public RunMethod(Action<object> method, Func<object, bool> canRun)
		{
			this.method = method;
			this.canRun = canRun;
		}

		public bool CanExecute(object parameter)
		{
			return canRun(parameter);
		}

		public void Execute(object parameter)
		{
			method(parameter);
		}
	}
}
