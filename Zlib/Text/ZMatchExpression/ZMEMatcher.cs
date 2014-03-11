using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Zlib.Algorithm;
using Zlib.Utility;

namespace Zlib.Text.ZMatchExpression {
    class ZMEMatcher {

        private IEnumerable<Piece> pieces;
        private IEnumerable<MatcherCapture> captures;
        private IEnumerable<MatcherCapture> icaptures;
        private IEnumerable<MatcherCapture> rcaptures;
        private Match match;
        private String input;
        private ZME zme;
        private ZMEMatch zmeMatch;

        private static readonly Comparison<Piece> comPiece = (l, r) => {
            var i = l.Index - r.Index;
            if (i != 0) return i;
            return l.Length - r.Length;
        };

        private static readonly Comparison<MatcherCapture> comCapture = (l, r) => {
            var i = l.Index - r.Index;
            if (i != 0) return i;
            return r.Length - l.Length;
        };

        private static readonly Regex R_SPACE = new Regex("\\s+");

        private void Split() {
            HashSet<int> ii = new HashSet<int>();
            HashSet<Piece> q = new HashSet<Piece>();
            foreach (Group g in match.Groups) {
                foreach (Capture c in g.Captures) {
                    ii.Add(c.Index);
                    ii.Add(c.Index + c.Length);
                }
            }
            var a = ii.QuickSort();

            for (int i = 1; i < a.Length; ++i) {
                Piece p = new Piece();
                p.Index = a[i - 1];
                p.Text = input.Substring(a[i - 1], a[i] - a[i - 1]);
                q.Add(p);
            }
            pieces = q.QuickSort(comPiece);
        }

        private void RebuildCaptures() {            
            var all = from c in
                           from gn in zme.Regex.GetGroupNames()
                           select new { C = match.Groups[gn].Captures, N = gn }
                               into cs
                               from c in cs.C.ToEnumerable<Capture>()
                               select new { C = c, N = cs.N }
                       select new MatcherCapture() {
                           Pieces = (from p in pieces where p.Index >= c.C.Index && p.Index < c.C.Index + c.C.Length orderby comCapture select p).ToArray(),
                           Group = c.N,
                           Index = c.C.Index,
                           Text = c.C.Value,
                           Parent = null
                       };
            captures = (from c in all where c.Length > 0 select c).ToArray();
            icaptures = (from c in all where c.Group.StartsWith("I_") select c).ToArray();
            rcaptures = (from c in captures where c.Group == "C" select c).ToArray();
        }

        private void ManageNumber() {            
            IEnumerable<MatcherCapture> ncs = from nc in captures where nc.Group == "N" select nc;            
            foreach (var nc in ncs) {
                nc.Numbers = new int[] { NumberUtil.ToNumber(nc.Text).Value };
                nc.Parent = (from c in captures
                             where c.Group == "C" && c.Index <= nc.Index && nc.Index < c.Index + c.Length
                             select c).First();
            }
            foreach (var cc in rcaptures) {
                cc.Numbers = (from nc in ncs
                             where nc.Index >= cc.Index && nc.Index < cc.Index + cc.Length
                             select nc.Numbers into ns
                             from n in ns
                             select n).ToArray();
            }
            
        }

        private void InsertInsertion() {            
            foreach (var ic in icaptures) {
                ic.Parent = (from c in rcaptures
                             where c.Index <= ic.Index && ic.Index + ic.Length <= c.Index + c.Length
                             select c).First();
                ic.Parent.Pieces = ic.Parent.Pieces.Union(ic.Pieces = new Piece[] { new Piece { Text = ic.Text, Index = ic.Index } }).OrderBy(comPiece).ToArray();
            }
        }

        private void ManagePiece() {
            foreach (var e in captures) {
                switch (e.Group) {
                    case "S": foreach (var p in e.Pieces) p.Text = " "; break;
                    case "D": foreach (var p in e.Pieces) p.Text = null; break;
                }
            }
            foreach (var e in icaptures) {
                foreach (var p in e.Pieces) {
                    p.Text = e.Group.Substring(2);
                }
            }
        }

        private void Recalc() {
            foreach (var c in captures) {
                c.Text = String.Join("", c.Pieces as IEnumerable<Piece>);
            }
        }

        private void BuildMatch(int depth) {
            zmeMatch = new ZMEMatch();
            zmeMatch.Captures = (from c in rcaptures select new ZMECapture { Numbers = c.Numbers, Text = R_SPACE.Replace(c.Text.Trim()," ") }).ToArray();
            zmeMatch.Text = String.Join(" ", zmeMatch.Captures as IEnumerable<ZMECapture>);
            zmeMatch.Depth = depth;
        }

        public bool IsMatch(ZME zme, String input) {
            this.input = input;
            this.zme = zme;
            if (zme.Children == null) {
                return zme.Regex.IsMatch(input);
            } else {
                foreach (var e in zme.Children) {
                    var r = new ZMEMatcher().IsMatch(e, input);
                    if (r) return true;
                }
                return false;
            }
        }

        public ZMEMatch Match(ZME zme, String input, int depth) {
            this.input = input;
            this.zme = zme;
            if (zme.Children == null) return RealMatch(depth);
            else {
                int i = 0;
                foreach (var e in zme.Children) {
                    var r = new ZMEMatcher().Match(e, input, depth + 1);
                    if (r != null) {
                        r.Position = i;
                        return r;
                    }
                    ++i;
                }
                return null;
            }
        }

        private ZMEMatch RealMatch(int depth) {
            match = zme.Regex.Match(input);
            if (!match.Success) return null;
            Split();
            RebuildCaptures();
            ManageNumber();
            InsertInsertion();
            ManagePiece();
            Recalc();
            BuildMatch(depth);
            return zmeMatch;
        }
    }
}
