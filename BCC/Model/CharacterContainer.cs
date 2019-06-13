using BCC.Commands;
using BCC.Data;
using BCC.Props;
using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BCC.Model
{
	class CharacterContainer : BaseProperty
	{
		public ObservableCollection<Character> Characters { get; }

		public ICommand Parse { get; }

		public CharacterContainer()
		{
			Characters = new ObservableCollection<Character>();
			Parse = new RunMethodAsync(ParseString, a => a is string);
		}

		public async Task ParseString(object parameter)
		{
			List<Character> characters;
			try
			{
				characters = ParseTextBox(parameter as string);
			}
			catch (Exception e)
			{
				MessageBox.Show($"Error: {e.Message}", "Error");
				return;
			}

			await App.Current.Dispatcher.Invoke(async () =>
			{
				Characters.Clear();
				for (int i = 0; i < characters.Count; i++)
					Characters.Add(characters[i]);

				OnPropertyChanged("SeatCharactersList");
				await Task.CompletedTask;
			});
		}

		List<Character> ParseTextBox(string content)
		{
			List<Character> characters = new List<Character>();
			string[] lines = content.Split('\n');

			Configuration config = new Configuration()
			{
				Delimiter = "\t",
				HeaderValidated = (isValid, b, c, d) =>
				{
					if (!isValid)
						throw new Exception("Header not found. Make sure you include headers in your tsv file.");
				},
				TrimOptions = TrimOptions.Trim,
			};

			using (var stream = StreamFromString(content))
			using (var csv = new CsvReader(new StreamReader(stream), config))
			{
				characters = csv.GetRecords<Character>().ToList();
			}

			return characters;
		}

		/// <summary>
		/// https://stackoverflow.com/questions/1879395/how-do-i-generate-a-stream-from-a-string
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		Stream StreamFromString(string str)
		{
			Stream stream = new MemoryStream();
			StreamWriter writer = new StreamWriter(stream);

			writer.Write(str);
			writer.Flush();
			stream.Position = 0;

			return stream;
		}
	}
}
