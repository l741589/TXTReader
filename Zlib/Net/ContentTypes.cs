﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zlib.Utility;
namespace Zlib.Net {
    class ContentTypes {

        private static Dictionary<String, String> dict;
        private static Dictionary<String, String> rdict;

        public static String GetContentType(String ext) {
            if (ext.IsNullOrWhiteSpace()) return "octet-stream";
            ext = ext.Trim().TrimStart('.').ToLower();
            if (dict.ContainsKey(ext)) return dict[ext];
            return "octet-stream";
        }

        public static String GetExt(String contentType) {
            if (contentType.IsNullOrWhiteSpace()) return null;
            contentType = contentType.Trim().ToLower();
            if (contentType == "octet-stream") return null;
            if (rdict.ContainsKey(contentType)) return rdict[contentType];
            return null;
        }

        public const String Form = "application/x-www-form-urlencoded";
        public const String FormData = "form-data";
        public const String MultipartFormData = "multipart/form-data";
        public const String MultipartMixed = "multipart/mixed";
        public const String MultipartAlternative = "multipart/alternative";
        public const String MultipartRelated = "multipart/related";

        static ContentTypes() {
            dict = new Dictionary<String, String>();
            dict.Add("3gp", "video/3gpp");
            dict.Add("aab", "application/x-authoware-bin");
            dict.Add("aam", "application/x-authoware-map");
            dict.Add("aas", "application/x-authoware-seg");
            dict.Add("ai", "application/postscript");
            dict.Add("aif", "audio/x-aiff");
            dict.Add("aifc", "audio/x-aiff");
            dict.Add("aiff", "audio/x-aiff");
            dict.Add("als", "audio/X-Alpha5");
            dict.Add("amc", "application/x-mpeg");
            dict.Add("ani", "application/octet-stream");
            dict.Add("asc", "text/plain");
            dict.Add("asd", "application/astound");
            dict.Add("asf", "video/x-ms-asf");
            dict.Add("asn", "application/astound");
            dict.Add("asp", "application/x-asap");
            dict.Add("asx", "video/x-ms-asf");
            dict.Add("au", "audio/basic");
            dict.Add("avb", "application/octet-stream");
            dict.Add("avi", "video/x-msvideo");
            dict.Add("awb", "audio/amr-wb");
            dict.Add("bcpio", "application/x-bcpio");
            dict.Add("bin", "application/octet-stream");
            dict.Add("bld", "application/bld");
            dict.Add("bld2", "application/bld2");
            dict.Add("bmp", "application/x-MS-bmp");
            dict.Add("bpk", "application/octet-stream");
            dict.Add("bz2", "application/x-bzip2");
            dict.Add("cal", "image/x-cals");
            dict.Add("ccn", "application/x-cnc");
            dict.Add("cco", "application/x-cocoa");
            dict.Add("cdf", "application/x-netcdf");
            dict.Add("cgi", "magnus-internal/cgi");
            dict.Add("chat", "application/x-chat");
            dict.Add("class", "application/octet-stream");
            dict.Add("clp", "application/x-msclip");
            dict.Add("cmx", "application/x-cmx");
            dict.Add("co", "application/x-cult3d-object");
            dict.Add("cod", "image/cis-cod");
            dict.Add("cpio", "application/x-cpio");
            dict.Add("cpt", "application/mac-compactpro");
            dict.Add("crd", "application/x-mscardfile");
            dict.Add("csh", "application/x-csh");
            dict.Add("csm", "chemical/x-csml");
            dict.Add("csml", "chemical/x-csml");
            dict.Add("css", "text/css");
            dict.Add("cur", "application/octet-stream");
            dict.Add("dcm", "x-lml/x-evm");
            dict.Add("dcr", "application/x-director");
            dict.Add("dcx", "image/x-dcx");
            dict.Add("dhtml", "text/html");
            dict.Add("dir", "application/x-director");
            dict.Add("dll", "application/octet-stream");
            dict.Add("dmg", "application/octet-stream");
            dict.Add("dms", "application/octet-stream");
            dict.Add("doc", "application/msword");
            dict.Add("dot", "application/x-dot");
            dict.Add("dvi", "application/x-dvi");
            dict.Add("dwf", "drawing/x-dwf");
            dict.Add("dwg", "application/x-autocad");
            dict.Add("dxf", "application/x-autocad");
            dict.Add("dxr", "application/x-director");
            dict.Add("ebk", "application/x-expandedbook");
            dict.Add("emb", "chemical/x-embl-dl-nucleotide");
            dict.Add("embl", "chemical/x-embl-dl-nucleotide");
            dict.Add("eps", "application/postscript");
            dict.Add("eri", "image/x-eri");
            dict.Add("es", "audio/echospeech");
            dict.Add("esl", "audio/echospeech");
            dict.Add("etc", "application/x-earthtime");
            dict.Add("etx", "text/x-setext");
            dict.Add("evm", "x-lml/x-evm");
            dict.Add("evy", "application/x-envoy");
            dict.Add("exe", "application/octet-stream");
            dict.Add("fh4", "image/x-freehand");
            dict.Add("fh5", "image/x-freehand");
            dict.Add("fhc", "image/x-freehand");
            dict.Add("fif", "image/fif");
            dict.Add("fm", "application/x-maker");
            dict.Add("fpx", "image/x-fpx");
            dict.Add("fvi", "video/isivideo");
            dict.Add("gau", "chemical/x-gaussian-input");
            dict.Add("gca", "application/x-gca-compressed");
            dict.Add("gdb", "x-lml/x-gdb");
            dict.Add("gif", "image/gif");
            dict.Add("gps", "application/x-gps");
            dict.Add("gtar", "application/x-gtar");
            dict.Add("gz", "application/x-gzip");
            dict.Add("hdf", "application/x-hdf");
            dict.Add("hdm", "text/x-hdml");
            dict.Add("hdml", "text/x-hdml");
            dict.Add("hlp", "application/winhlp");
            dict.Add("hqx", "application/mac-binhex40");
            dict.Add("htm", "text/html");
            dict.Add("html", "text/html");
            dict.Add("hts", "text/html");
            dict.Add("ice", "x-conference/x-cooltalk");
            dict.Add("ico", "application/octet-stream");
            dict.Add("ief", "image/ief");
            dict.Add("ifm", "image/gif");
            dict.Add("ifs", "image/ifs");
            dict.Add("imy", "audio/melody");
            dict.Add("ins", "application/x-NET-Install");
            dict.Add("ips", "application/x-ipscript");
            dict.Add("ipx", "application/x-ipix");
            dict.Add("it", "audio/x-mod");
            dict.Add("itz", "audio/x-mod");
            dict.Add("ivr", "i-world/i-vrml");
            dict.Add("j2k", "image/j2k");
            dict.Add("jad", "text/vnd.sun.j2me.app-descriptor");
            dict.Add("jam", "application/x-jam");
            dict.Add("jar", "application/java-archive");
            dict.Add("jnlp", "application/x-java-jnlp-file");
            dict.Add("jpe", "image/jpeg");
            dict.Add("jpeg", "image/jpeg");
            dict.Add("jpg", "image/jpeg");
            dict.Add("jpz", "image/jpeg");
            dict.Add("js", "application/x-javascript");
            dict.Add("jwc", "application/jwc");
            dict.Add("kjx", "application/x-kjx");
            dict.Add("lak", "x-lml/x-lak");
            dict.Add("latex", "application/x-latex");
            dict.Add("lcc", "application/fastman");
            dict.Add("lcl", "application/x-digitalloca");
            dict.Add("lcr", "application/x-digitalloca");
            dict.Add("lgh", "application/lgh");
            dict.Add("lha", "application/octet-stream");
            dict.Add("lml", "x-lml/x-lml");
            dict.Add("lmlpack", "x-lml/x-lmlpack");
            dict.Add("lsf", "video/x-ms-asf");
            dict.Add("lsx", "video/x-ms-asf");
            dict.Add("lzh", "application/x-lzh");
            dict.Add("m13", "application/x-msmediaview");
            dict.Add("m14", "application/x-msmediaview");
            dict.Add("m15", "audio/x-mod");
            dict.Add("m3u", "audio/x-mpegurl");
            dict.Add("m3url", "audio/x-mpegurl");
            dict.Add("ma1", "audio/ma1");
            dict.Add("ma2", "audio/ma2");
            dict.Add("ma3", "audio/ma3");
            dict.Add("ma5", "audio/ma5");
            dict.Add("man", "application/x-troff-man");
            dict.Add("map", "magnus-internal/imagemap");
            dict.Add("mbd", "application/mbedlet");
            dict.Add("mct", "application/x-mascot");
            dict.Add("mdb", "application/x-msaccess");
            dict.Add("mdz", "audio/x-mod");
            dict.Add("me", "application/x-troff-me");
            dict.Add("mel", "text/x-vmel");
            dict.Add("mi", "application/x-mif");
            dict.Add("mid", "audio/midi");
            dict.Add("midi", "audio/midi");
            dict.Add("mif", "application/x-mif");
            dict.Add("mil", "image/x-cals");
            dict.Add("mio", "audio/x-mio");
            dict.Add("mmf", "application/x-skt-lbs");
            dict.Add("mng", "video/x-mng");
            dict.Add("mny", "application/x-msmoney");
            dict.Add("moc", "application/x-mocha");
            dict.Add("mocha", "application/x-mocha");
            dict.Add("mod", "audio/x-mod");
            dict.Add("mof", "application/x-yumekara");
            dict.Add("mol", "chemical/x-mdl-molfile");
            dict.Add("mop", "chemical/x-mopac-input");
            dict.Add("mov", "video/quicktime");
            dict.Add("movie", "video/x-sgi-movie");
            dict.Add("mp2", "audio/x-mpeg");
            dict.Add("mp3", "audio/x-mpeg");
            dict.Add("mp4", "video/mp4");
            dict.Add("mpc", "application/vnd.mpohun.certificate");
            dict.Add("mpe", "video/mpeg");
            dict.Add("mpeg", "video/mpeg");
            dict.Add("mpg", "video/mpeg");
            dict.Add("mpg4", "video/mp4");
            dict.Add("mpga", "audio/mpeg");
            dict.Add("mpn", "application/vnd.mophun.application");
            dict.Add("mpp", "application/vnd.ms-project");
            dict.Add("mps", "application/x-mapserver");
            dict.Add("mrl", "text/x-mrml");
            dict.Add("mrm", "application/x-mrm");
            dict.Add("ms", "application/x-troff-ms");
            dict.Add("mts", "application/metastream");
            dict.Add("mtx", "application/metastream");
            dict.Add("mtz", "application/metastream");
            dict.Add("mzv", "application/metastream");
            dict.Add("nar", "application/zip");
            dict.Add("nbmp", "image/nbmp");
            dict.Add("nc", "application/x-netcdf");
            dict.Add("ndb", "x-lml/x-ndb");
            dict.Add("ndwn", "application/ndwn");
            dict.Add("nif", "application/x-nif");
            dict.Add("nmz", "application/x-scream");
            dict.Add("nokia-op-logo", "image/vnd.nok-oplogo-color");
            dict.Add("npx", "application/x-netfpx");
            dict.Add("nsnd", "audio/nsnd");
            dict.Add("nva", "application/x-neva1");
            dict.Add("oda", "application/oda");
            dict.Add("oom", "application/x-AtlasMate-Plugin");
            dict.Add("pac", "audio/x-pac");
            dict.Add("pae", "audio/x-epac");
            dict.Add("pan", "application/x-pan");
            dict.Add("pbm", "image/x-portable-bitmap");
            dict.Add("pcx", "image/x-pcx");
            dict.Add("pda", "image/x-pda");
            dict.Add("pdb", "chemical/x-pdb");
            dict.Add("pdf", "application/pdf");
            dict.Add("pfr", "application/font-tdpfr");
            dict.Add("pgm", "image/x-portable-graymap");
            dict.Add("pict", "image/x-pict");
            dict.Add("pm", "application/x-perl");
            dict.Add("pmd", "application/x-pmd");
            dict.Add("png", "image/png");
            dict.Add("pnm", "image/x-portable-anymap");
            dict.Add("pnz", "image/png");
            dict.Add("pot", "application/vnd.ms-powerpoint");
            dict.Add("ppm", "image/x-portable-pixmap");
            dict.Add("pps", "application/vnd.ms-powerpoint");
            dict.Add("ppt", "application/vnd.ms-powerpoint");
            dict.Add("pqf", "application/x-cprplayer");
            dict.Add("pqi", "application/cprplayer");
            dict.Add("prc", "application/x-prc");
            dict.Add("proxy", "application/x-ns-proxy-autoconfig");
            dict.Add("ps", "application/postscript");
            dict.Add("ptlk", "application/listenup");
            dict.Add("pub", "application/x-mspublisher");
            dict.Add("pvx", "video/x-pv-pvx");
            dict.Add("qcp", "audio/vnd.qcelp");
            dict.Add("qt", "video/quicktime");
            dict.Add("qti", "image/x-quicktime");
            dict.Add("qtif", "image/x-quicktime");
            dict.Add("r3t", "text/vnd.rn-realtext3d");
            dict.Add("ra", "audio/x-pn-realaudio");
            dict.Add("ram", "audio/x-pn-realaudio");
            dict.Add("rar", "application/x-rar-compressed");
            dict.Add("ras", "image/x-cmu-raster");
            dict.Add("rdf", "application/rdf+xml");
            dict.Add("rf", "image/vnd.rn-realflash");
            dict.Add("rgb", "image/x-rgb");
            dict.Add("rlf", "application/x-richlink");
            dict.Add("rm", "audio/x-pn-realaudio");
            dict.Add("rmf", "audio/x-rmf");
            dict.Add("rmm", "audio/x-pn-realaudio");
            dict.Add("rmvb", "audio/x-pn-realaudio");
            dict.Add("rnx", "application/vnd.rn-realplayer");
            dict.Add("roff", "application/x-troff");
            dict.Add("rp", "image/vnd.rn-realpix");
            dict.Add("rpm", "audio/x-pn-realaudio-plugin");
            dict.Add("rt", "text/vnd.rn-realtext");
            dict.Add("rte", "x-lml/x-gps");
            dict.Add("rtf", "application/rtf");
            dict.Add("rtg", "application/metastream");
            dict.Add("rtx", "text/richtext");
            dict.Add("rv", "video/vnd.rn-realvideo");
            dict.Add("rwc", "application/x-rogerwilco");
            dict.Add("s3m", "audio/x-mod");
            dict.Add("s3z", "audio/x-mod");
            dict.Add("sca", "application/x-supercard");
            dict.Add("scd", "application/x-msschedule");
            dict.Add("sdf", "application/e-score");
            dict.Add("sea", "application/x-stuffit");
            dict.Add("sgm", "text/x-sgml");
            dict.Add("sgml", "text/x-sgml");
            dict.Add("sh", "application/x-sh");
            dict.Add("shar", "application/x-shar");
            dict.Add("shtml", "magnus-internal/parsed-html");
            dict.Add("shw", "application/presentations");
            dict.Add("si6", "image/si6");
            dict.Add("si7", "image/vnd.stiwap.sis");
            dict.Add("si9", "image/vnd.lgtwap.sis");
            dict.Add("sis", "application/vnd.symbian.install");
            dict.Add("sit", "application/x-stuffit");
            dict.Add("skd", "application/x-Koan");
            dict.Add("skm", "application/x-Koan");
            dict.Add("skp", "application/x-Koan");
            dict.Add("skt", "application/x-Koan");
            dict.Add("slc", "application/x-salsa");
            dict.Add("smd", "audio/x-smd");
            dict.Add("smi", "application/smil");
            dict.Add("smil", "application/smil");
            dict.Add("smp", "application/studiom");
            dict.Add("smz", "audio/x-smd");
            dict.Add("snd", "audio/basic");
            dict.Add("spc", "text/x-speech");
            dict.Add("spl", "application/futuresplash");
            dict.Add("spr", "application/x-sprite");
            dict.Add("sprite", "application/x-sprite");
            dict.Add("spt", "application/x-spt");
            dict.Add("src", "application/x-wais-source");
            dict.Add("stk", "application/hyperstudio");
            dict.Add("stm", "audio/x-mod");
            dict.Add("sv4cpio", "application/x-sv4cpio");
            dict.Add("sv4crc", "application/x-sv4crc");
            dict.Add("svf", "image/vnd");
            dict.Add("svg", "image/svg-xml");
            dict.Add("svh", "image/svh");
            dict.Add("svr", "x-world/x-svr");
            dict.Add("swf", "application/x-shockwave-flash");
            dict.Add("swfl", "application/x-shockwave-flash");
            dict.Add("t", "application/x-troff");
            dict.Add("tad", "application/octet-stream");
            dict.Add("talk", "text/x-speech");
            dict.Add("tar", "application/x-tar");
            dict.Add("taz", "application/x-tar");
            dict.Add("tbp", "application/x-timbuktu");
            dict.Add("tbt", "application/x-timbuktu");
            dict.Add("tcl", "application/x-tcl");
            dict.Add("tex", "application/x-tex");
            dict.Add("texi", "application/x-texinfo");
            dict.Add("texinfo", "application/x-texinfo");
            dict.Add("tgz", "application/x-tar");
            dict.Add("thm", "application/vnd.eri.thm");
            dict.Add("tif", "image/tiff");
            dict.Add("tiff", "image/tiff");
            dict.Add("tki", "application/x-tkined");
            dict.Add("tkined", "application/x-tkined");
            dict.Add("toc", "application/toc");
            dict.Add("toy", "image/toy");
            dict.Add("tr", "application/x-troff");
            dict.Add("trk", "x-lml/x-gps");
            dict.Add("trm", "application/x-msterminal");
            dict.Add("tsi", "audio/tsplayer");
            dict.Add("tsp", "application/dsptype");
            dict.Add("tsv", "text/tab-separated-values");
            dict.Add("ttf", "application/octet-stream");
            dict.Add("ttz", "application/t-time");
            dict.Add("txt", "text/plain");
            dict.Add("ult", "audio/x-mod");
            dict.Add("ustar", "application/x-ustar");
            dict.Add("uu", "application/x-uuencode");
            dict.Add("uue", "application/x-uuencode");
            dict.Add("vcd", "application/x-cdlink");
            dict.Add("vcf", "text/x-vcard");
            dict.Add("vdo", "video/vdo");
            dict.Add("vib", "audio/vib");
            dict.Add("viv", "video/vivo");
            dict.Add("vivo", "video/vivo");
            dict.Add("vmd", "application/vocaltec-media-desc");
            dict.Add("vmf", "application/vocaltec-media-file");
            dict.Add("vmi", "application/x-dreamcast-vms-info");
            dict.Add("vms", "application/x-dreamcast-vms");
            dict.Add("vox", "audio/voxware");
            dict.Add("vqe", "audio/x-twinvq-plugin");
            dict.Add("vqf", "audio/x-twinvq");
            dict.Add("vql", "audio/x-twinvq");
            dict.Add("vre", "x-world/x-vream");
            dict.Add("vrml", "x-world/x-vrml");
            dict.Add("vrt", "x-world/x-vrt");
            dict.Add("vrw", "x-world/x-vream");
            dict.Add("vts", "workbook/formulaone");
            dict.Add("wav", "audio/x-wav");
            dict.Add("wax", "audio/x-ms-wax");
            dict.Add("wbmp", "image/vnd.wap.wbmp");
            dict.Add("web", "application/vnd.xara");
            dict.Add("wi", "image/wavelet");
            dict.Add("wis", "application/x-InstallShield");
            dict.Add("wm", "video/x-ms-wm");
            dict.Add("wma", "audio/x-ms-wma");
            dict.Add("wmd", "application/x-ms-wmd");
            dict.Add("wmf", "application/x-msmetafile");
            dict.Add("wml", "text/vnd.wap.wml");
            dict.Add("wmlc", "application/vnd.wap.wmlc");
            dict.Add("wmls", "text/vnd.wap.wmlscript");
            dict.Add("wmlsc", "application/vnd.wap.wmlscriptc");
            dict.Add("wmlscript", "text/vnd.wap.wmlscript");
            dict.Add("wmv", "audio/x-ms-wmv");
            dict.Add("wmx", "video/x-ms-wmx");
            dict.Add("wmz", "application/x-ms-wmz");
            dict.Add("wpng", "image/x-up-wpng");
            dict.Add("wpt", "x-lml/x-gps");
            dict.Add("wri", "application/x-mswrite");
            dict.Add("wrl", "x-world/x-vrml");
            dict.Add("wrz", "x-world/x-vrml");
            dict.Add("ws", "text/vnd.wap.wmlscript");
            dict.Add("wsc", "application/vnd.wap.wmlscriptc");
            dict.Add("wv", "video/wavelet");
            dict.Add("wvx", "video/x-ms-wvx");
            dict.Add("wxl", "application/x-wxl");
            dict.Add("x-gzip", "application/x-gzip");
            dict.Add("xar", "application/vnd.xara");
            dict.Add("xbm", "image/x-xbitmap");
            dict.Add("xdm", "application/x-xdma");
            dict.Add("xdma", "application/x-xdma");
            dict.Add("xdw", "application/vnd.fujixerox.docuworks");
            dict.Add("xht", "application/xhtml+xml");
            dict.Add("xhtm", "application/xhtml+xml");
            dict.Add("xhtml", "application/xhtml+xml");
            dict.Add("xla", "application/vnd.ms-excel");
            dict.Add("xlc", "application/vnd.ms-excel");
            dict.Add("xll", "application/x-excel");
            dict.Add("xlm", "application/vnd.ms-excel");
            dict.Add("xls", "application/vnd.ms-excel");
            dict.Add("xlt", "application/vnd.ms-excel");
            dict.Add("xlw", "application/vnd.ms-excel");
            dict.Add("xm", "audio/x-mod");
            dict.Add("xml", "text/xml");
            dict.Add("xmz", "audio/x-mod");
            dict.Add("xpi", "application/x-xpinstall");
            dict.Add("xpm", "image/x-xpixmap");
            dict.Add("xsit", "text/xml");
            dict.Add("xsl", "text/xml");
            dict.Add("xul", "text/xul");
            dict.Add("xwd", "image/x-xwindowdump");
            dict.Add("xyz", "chemical/x-pdb");
            dict.Add("yz1", "application/x-yz1");
            dict.Add("z", "application/x-compress");
            dict.Add("zac", "application/x-zaurus-zac");
            dict.Add("zip", "application/zip*/");

            rdict = new Dictionary<string, string>();
            foreach (var e in dict) {
                if (!rdict.ContainsKey(e.Value))
                    rdict.Add(e.Value, e.Key);
            }
        }
    }
}
