using System.Collections.Generic;

namespace Farsaga.GameClasses.MapClasses {

    public class TiledMap {

        public Tile[,] tiles;

        public TiledMap() {

        }
        
        public void SetTiles(Tile[,] tiles) {
            this.tiles = tiles;
        }
    }

}
