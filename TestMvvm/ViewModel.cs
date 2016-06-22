using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMvvm.Core;
using System.ComponentModel;
using System.Windows.Input;
using System.IO;

namespace TestMvvm
{
    class ViewModel : ViewModelBase
    {
        // attributi
        // rappresentano i dati utilizzati nell’applicazione
        // provenienti dalle classi del Model

        private Model _modello;
        private string _messaggio;
        // Properties
        // rappresentano le informazioni visualizzate nella MainWindow
        // associate agli elementi dell’interfaccia grafica
        // esse si ricavano dai dati dichiarati come attributi di questa classe

        public ViewModel()
            {
            Modello = new Model { Contenuto= "asdf", Counter= 1 , Words= new List<string>(), Selected = new List<string>() };
            Messaggio = "Init";

            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader("dictionary.txt"))
                {
                    //inizzializzo l'array di parole
                    string all = sr.ReadToEnd();
                    string[] words = all.Split('\n');
                    foreach ( string word in words)
                    {
                        Modello.Words.Add(word);
                    }
                    // svuoto la stringa visto che non mi serve più
                    all = "";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

        }
        
        public Model Modello
        {
            get { return _modello; }

            set
            {
                _modello = value;
                OnPropertyChanged("Modello");
            }
        }

        public String Messaggio
        {
            get { return _messaggio; }

            set
            {
                _messaggio = value;
                OnPropertyChanged("Messaggio");
            }
        }


        /* vecchio comando
        public ICommandNoView ModificaMessaggio
        {
            get { return new RelayCommandNoView(ModificaMessaggioExecute, CanModificaMessaggioExecute); }
        }

        private bool CanModificaMessaggioExecute(object obj)
        {
            return true;
        }

        private void ModificaMessaggioExecute(object obj)
        {
            Messaggio = "Modificato!";
        }
        */

        #region Command ModificaMessaggio

        public ICommand ModificaMessaggio
        {
            get { return new RelayCommand(ModificaMessaggioExecute, CanModificaMessaggioExecute); }
        }

        private bool CanModificaMessaggioExecute(object obj)
        {
            return true;
        }

        private void ModificaMessaggioExecute(object obj)
        {

            var searchQuery = from word in Modello.Words
                              where word.Contains(Messaggio)
                              select word;
            try
            {
                Modello.Selected = new List<string>();
                foreach (string qwerty in searchQuery)
                {
                    Modello.Selected.Add(qwerty);
                }
            }
            catch
            {

            }
        }

        #endregion

        #region Command Allwords

        public ICommand AllWords
        {
            get { return new RelayCommand(AllWordsExecute, CanAllWordsExecute); }
        }

        private bool CanAllWordsExecute(object obj)
        {
            return true;
        }

        private void AllWordsExecute(object obj)
        {
            Modello.Selected = Modello.Words;
            OnPropertyChanged("Modello");
        }

        #endregion
    }
}
