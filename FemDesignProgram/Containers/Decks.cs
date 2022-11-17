using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FemDesignProgram.Containers
{
    public class Decks
    {

        public List<object> DecksInModel = new List<object>();

        public void AddDeck(Deck deck)
        {
            DecksInModel.Add(deck);
        }
    }
}
