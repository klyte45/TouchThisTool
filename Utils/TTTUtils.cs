using Klyte.Commons.Utils;
using System;
using System.Linq;
using UnityEngine;

namespace Klyte.TouchThis.Utils
{
    internal class TTTUtils : KlyteUtils
    {

        #region Logging
        public static void doLog(string format, params object[] args)
        {
            try
            {
                if (TouchThisToolMod.debugMode)
                {
                    if (TouchThisToolMod.instance != null)
                    {
                        Debug.LogWarningFormat("TTTRv" + TouchThisToolMod.version + " " + format, args);

                    }
                    else
                    {
                        Console.WriteLine("TTTRv" + TouchThisToolMod.version + " " + format, args);
                    }
                }
            }
            catch
            {
                Debug.LogErrorFormat("TTTRv" + TouchThisToolMod.version + " Erro ao fazer log: {0} (args = {1})", format, args == null ? "[]" : string.Join(",", args.Select(x => x != null ? x.ToString() : "--NULL--").ToArray()));
            }
        }
        public static void doErrorLog(string format, params object[] args)
        {
            try
            {
                if (TouchThisToolMod.instance != null)
                {
                    Debug.LogErrorFormat("TTTRv" + TouchThisToolMod.version + " " + format, args);
                }
                else
                {
                    Console.WriteLine("TTTRv" + TouchThisToolMod.version + " " + format, args);
                }
            }
            catch
            {
                Debug.LogErrorFormat("TTTRv" + TouchThisToolMod.version + " Erro ao logar ERRO!!!: {0} (args = [{1}])", format, args == null ? "" : string.Join(",", args.Select(x => x != null ? x.ToString() : "--NULL--").ToArray()));
            }

        }
        #endregion


    }

}

