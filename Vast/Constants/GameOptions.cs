using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Farsaga.Util;

namespace Farsaga.Constants {

    public class GameOptions {

        private static GameOptions instance;

        public static GameOptions Instance {
            get {
                if (instance == null) {
                    instance = new GameOptions();
                }
                return instance;
            }
        }

        public float MUSIC_VOLUME = 0.5f;
        public float EFFECT_VOLUME = 1.0f;

        public void SetOptions() {
            MUSIC_VOLUME = float.Parse(ContentChest.Instance.options.DocumentElement.SelectSingleNode("musicvol").InnerText);
            EFFECT_VOLUME = float.Parse(ContentChest.Instance.options.DocumentElement.SelectSingleNode("effectvol").InnerText);
        }

    }

}
