using ColossalFramework.Globalization;
using Klyte.TouchThis.Utils;
using Klyte.Commons.i18n;

using System;

namespace Klyte.TouchThis.i18n
{
    public class TTTLocaleUtils : KlyteLocaleUtils<TTTLocaleUtils, TTTResourceLoader>
    {
        public override string prefix => "TTT_";

        protected override string packagePrefix => "Klyte.TouchThis";
    }
}
