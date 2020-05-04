using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirusBonker.GameSystem.UncontrollableGameObjects
{
    public class GameObjectOfBodyInfo
    {
        public string Name { get; set; }

        public IGameObject GameObject { get; set; }

        public GameObjectOfBodyInfo(string name, IGameObject gameObject)
        {
            Name = name;
            GameObject = gameObject;
        }
    }

    public interface IGameObject { }
}
