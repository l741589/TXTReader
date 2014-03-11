using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Zlib.Text.ZSpiderScript {
    internal class JudgeUnit {
        public delegate String VarGenerator(String input);
        public enum Operators { Equal, Match, StartWith, EndWith, Contains, Empty };

        public String LHS { get; set; }
        public String RHS { get; set; }
        public Operators OP { get; set; }
        public VarGenerator GenVar { get; set; }

        public JudgeUnit(List<String> Arguments, VarGenerator genVar) {
            String op = "equal";
            GenVar = genVar;
            if (Arguments == null || Arguments.Count == 0) throw new ZSSParseException("No Enough Arguments");
            switch (Arguments.Count) {
                case 1: LHS = null; op = Arguments[0]; break;
                case 2: LHS = null; op = Arguments[0]; RHS = Arguments[1]; break;
                default: LHS = Arguments[0]; op = Arguments[1]; RHS = Arguments[2]; break;
            }
            switch (op.ToLower()) {
                case "=":
                case "==":
                case "eq":
                case "equal":
                case "equals": OP = Operators.Equal; break;
                case "m":
                case "mat":
                case "match":
                case "matches": OP = Operators.Match; break;
                case "s":
                case "sw":
                case "start":
                case "starts":                
                case "startwith":
                case "startswith": OP = Operators.StartWith; break;
                case "e":
                case "ew":
                case "ends":
                case "endwith":
                case "endswith": OP = Operators.EndWith; break;
                case "c":
                case "con":
                case "contain":
                case "contains": OP = Operators.Contains; break;
                case "empty": OP = Operators.Empty; break;
                default: throw new ZSSParseException("Invalid Operator");
            }
            if (OP != Operators.Empty && RHS == null) throw new ZSSParseException("No Enough Parameters");
        }

        public bool Execute(String input, bool not = false) {
            if (LHS == null) return Execute(input, RHS, OP, GenVar, not);
            else return Execute(LHS, RHS, OP, GenVar, not);
        }

        public static bool Execute(String LHS, String RHS, Operators Op, VarGenerator gen, bool not = false) {
            if (LHS == null || (RHS == null&&Op!=Operators.Empty) || gen == null) throw new ZSSRuntimeException("Invalid Parameters");
            LHS = gen(LHS);
            RHS = gen(RHS);
            switch (Op) {
                case Operators.Equal: return (LHS == RHS) ^ not;
                case Operators.Match: return Regex.IsMatch(LHS, RHS)^not;
                case Operators.StartWith: return LHS.StartsWith(RHS) ^ not;
                case Operators.EndWith: return LHS.EndsWith(RHS) ^ not;
                case Operators.Contains: return LHS.Contains(RHS) ^ not;
                case Operators.Empty: return String.IsNullOrWhiteSpace(LHS) ^ not;
                default: throw new ZSSRuntimeException("Invalid Parameters");
            }
        }
    }
}