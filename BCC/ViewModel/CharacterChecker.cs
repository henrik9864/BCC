using BCC.Commands;
using BCC.Data;
using BCC.Model;
using BCC.Props;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BCC.ViewModel
{
	class CharacterChecker : BaseProperty
	{
		public ObservableCollection<Character> Characters { get; } = new ObservableCollection<Character>();

		public CharacterContainer GsfContainer { get; }

		public CharacterContainer SeatContainer { get; }

		public CharacterContainer DiscordContainer { get; }

		public ICommand Compare { get; }

		public ICommand LoadFile { get; }

		public ICommand DropFile { get; }

		public CharacterChecker()
		{
			GsfContainer = new CharacterContainer();
			SeatContainer = new CharacterContainer();
			DiscordContainer = new CharacterContainer();

			Compare = new RunMethod(CompareList, a => true);
			LoadFile = new RunMethodAsync(LoadDataFile, a => true);
			DropFile = new RunMethodAsync(async a =>
			{
				string[] filePaths = a as string[];
				await LoadFiles(filePaths);
			}, a => true);
		}

		void CompareList(object parameter)
		{
			Characters.Clear();
			for (int i = 0; i < GsfContainer.Characters.Count; i++)
			{
				Character character = GsfContainer.Characters[i];
				bool added = false;

				if (!SeatContainer.Characters.Any(a => a.Name.Replace(" ", "") == character.Name.Replace(" ", "")))
				{
					Characters.Add(character);
					character.Seat = false;
					added = true;
				}

				if (character.Main == character.Name && DiscordContainer.Characters.Count > 0 && !DiscordContainer.Characters.Any(a => a.Name.Replace(" ", "") == character.Main.Replace(" ", "")))
				{
					character.Discord = false;

					if (!added)
						Characters.Add(character);
				}
			}

			OnPropertyChanged("Characters");
		}

		async Task LoadDataFile(object parameter)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.DefaultExt = ".tsv";
			dlg.Multiselect = true;

			bool? result = dlg.ShowDialog();

			if (result == true)
				await LoadFiles(dlg.FileNames);
		}

		async Task LoadFiles(string[] fileNames)
		{
			foreach (string filePath in fileNames)
			{
				string name = Path.GetFileNameWithoutExtension(filePath);
				using (StreamReader reader = new StreamReader(filePath))
				{
					string content = await reader.ReadToEndAsync();

					if (name.ToLower() == "gsf")
						await GsfContainer.ParseString(content);

					if (name.ToLower() == "seat")
						await SeatContainer.ParseString(content);

					if (name.ToLower() == "discord")
						await DiscordContainer.ParseString(content);
				}
			}

			if (GsfContainer.Characters.Count > 0
				&& SeatContainer.Characters.Count > 0
				&& DiscordContainer.Characters.Count > 0)
			{
				await App.Current.Dispatcher.Invoke(async () =>
				{
					CompareList(null);
					await Task.CompletedTask;
				});
			}
		}
	}
}
