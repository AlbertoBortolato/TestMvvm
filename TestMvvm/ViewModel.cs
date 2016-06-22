using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestMvvm.Core;
using System.ComponentModel;
using System.Windows.Input;

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
            Modello = new Model { Contenuto= "asdf", Counter=1 };
            Messaggio = "Init";
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
            Messaggio = "Modificato!"+Modello.Counter;
            Modello.Counter++;
        }
    }
}
