using BCC.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BCC.Commands
{
	class RunMethodAsync : ICommandAsync
	{
		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		Func<object, Task> method;
		Func<object, bool> canRun;

		public RunMethodAsync(Func<object, Task> method, Func<object, bool> canRun)
		{
			this.method = method;
			this.canRun = canRun;
		}

		public void RaiseCanExecuteChanged()
		{
			CommandManager.InvalidateRequerySuggested();
		}

		public bool CanExecute(object parameter)
		{
			return canRun(parameter);
		}

		public async void Execute(object parameter)
		{
			await method(parameter);
		}

	}
}
