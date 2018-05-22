using System;
using UnityEngine;

namespace Klyte.TouchThis
{

    public class KeyBinding
    {
        private KeyCode[] key;

        public string toString()
        {
            return ((this.key[0] != KeyCode.None) ? this.key[0].ToString() : "") + ((this.key[1] != KeyCode.None) ? (" " + this.key[1].ToString()) : "") + ((this.key[2] != KeyCode.None) ? (" " + this.key[2].ToString()) : "");
        }

        public KeyBinding(KeyCode key1, KeyCode key2, KeyCode key3)
        {
            KeyCode[] array = new KeyCode[3];
            this.key = array;            
            this.key[0] = key1;
            this.key[1] = key2;
            this.key[2] = key3;
        }

        public KeyBinding(string str)
        {
            KeyCode[] array = new KeyCode[3];
            this.key = array;
            char[] separator = new char[]
            {
            ' ',
            '\t'
            };
            string[] array2 = str.Trim().Split(separator);
            int num = Math.Min(this.key.Length, array2.Length);
            for (int i = 0; i < num; i++)
            {
                if (Enum.IsDefined(typeof(KeyCode), array2[i].Trim()))
                {
                    this.key[i] = (KeyCode)Enum.Parse(typeof(KeyCode), array2[i]);
                }
            }
        }

        public bool isPressed()
        {
            if (this.key[0] == KeyCode.None)
            {
                return false;
            }
            for (int i = 0; i < 3; i++)
            {
                if (this.key[i] != KeyCode.None)
                {
                    if (i == 2 || this.key[i + 1] == KeyCode.None)
                    {
                        if (!Input.GetKeyDown(this.key[i]))
                        {
                            return false;
                        }
                    }
                    else if (!Input.GetKey(this.key[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

    }
}
