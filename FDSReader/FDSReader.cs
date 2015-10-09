using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Web.Script;

namespace FDSReader
{
    public static class FDSReader
    {
        public static string FileName;

        static double MinX = 0, MinY = 0, MinZ = 0, MaxX = 0, MaxY = 0, MaxZ = 0;

        public static string[] Reader()
        {
            string[] info = File.ReadAllLines(FileName);

            List<string> list = new List<string>();
            string str = string.Empty;
            foreach(string data in info)
            {
                string org = data.Trim();
                if (org == string.Empty) continue;

               // if(org.Substring(0,1)=="&")
                {
                    str += org+" ";
                }

                if (str.IndexOf("/") >= 0)
                {
                    list.Add(str);
                    str = string.Empty;
                }


            }


            return list.ToArray();
        }


        public struct struCommand
        {
            public CommandEnum cmdEnum;
            public long LineNUM;//行号
            public Guid ID;//内部ID
            public string CMDID;//对象ID
            public string strCommand;//命令
            public string Data;//原始数据
            public string Note;//注释
            public Dictionary<string, string> Parameters;
            public string c;
            public Point p1;//坐标点1
            public Point p2;//坐标点2
            public int isShow; //-1:不详   0：无需显示，需配置    1：显示相关，需配置   2：无需显示，无需配置
        }

        public struct CommandType
        {
            public CommandEnum cmdEnum;//命令描述
            public Guid ID;//命令GUID
            public string Command;//命令名称
            public string CommandNote;//命令解释
            public Dictionary<string, string> Parameters;//命令参数
            public string Demo;//命令示例
            public string Memory;//命令备注
            public int isShow; //-1:不详   0：无需显示   1：需要显示
            public int isConfig;//-1:不详  0：无需配置   1：需要配置
        }

        public static string GetCommandNote()
        {
            string Note = string.Empty;
            List<CommandType> CTList = new List<CommandType>();
            CommandType CT = new CommandType();
            CT.ID = Guid.Parse("8D9A932B-A9E5-4C5D-A372-ADEAA853A278");
            CT.cmdEnum = CommandEnum.HEAD;
            CT.Command = CommandEnum.HEAD.ToString();
            CT.CommandNote = "FDS文件的基础设置，需放置在第一行.";
            CT.Demo = "&HEAD CHID='WTC_05_v5', TITLE='WTC Phase 1, Test 5, FDS version 5' /";
            CT.isShow = 0;
            CT.isConfig = 1;
            CT.Parameters = new Dictionary<string, string>();
            CT.Parameters.Add("CHID", "char(30)|定义输出文件名");
            CT.Parameters.Add("TITLE", "char(60)|定义文件描述");
            CTList.Add(CT);

            CT = new CommandType();
            CT.ID = Guid.Parse("EE575945-9FBF-412C-BF69-3BC5B7163B38");
            CT.cmdEnum = CommandEnum.TIME;
            CT.Command = CommandEnum.TIME.ToString();
            CT.CommandNote = "定义了模拟持续的时间和初始时间步骤";
            CT.Demo = "&TIME T_END=900.0/";
            CT.isShow = 0;
            CT.isConfig = 1;
            CT.Parameters = new Dictionary<string, string>();
            CT.Parameters.Add("T_BEGIN", "int|定义模拟开始时间");
            CT.Parameters.Add("T_END", "int|定义模拟持续时间");
            CT.Parameters.Add("SYNCHRONIZE", "bool|TRUE:允许FDS自动改变时间步长\r\nFALSE:不允许FDS自动改变时间步长");
            CT.Memory = "通常只需要定义T_END";
            CTList.Add(CT);

            CT = new CommandType();
            CT.ID = Guid.Parse("3FF29880-320E-4D57-90FD-DA44E9DBE154");
            CT.cmdEnum = CommandEnum.DUMP;
            CT.Command = CommandEnum.DUMP.ToString();
            CT.CommandNote = "控制输出文件产生速度的参数";
            CT.Demo = "&DUMP RENDER_FILE='MP02-80.ge1', DT_RESTART=300.0/";
            CT.isShow = 0;
            CT.isConfig = 1;
            CT.Parameters = new Dictionary<string, string>();
            CT.Parameters.Add("RENDER_FILE", "char(30)|指定输出渲染文件名");
            CT.Parameters.Add("DT_RESTART", "double|不详");
            CT.Memory = "";
            CTList.Add(CT);

            CT = new CommandType();
            CT.ID = Guid.Parse("A39C0616-9DDA-4D1C-BC93-2060E1FBF2E8");
            CT.cmdEnum = CommandEnum.MISC;
            CT.Command = CommandEnum.MISC.ToString();
            CT.CommandNote = "控制输出文件产生速度的参数";
            CT.Demo = "&MISC SURF_DEFAULT='CONCRETE',TMPA=25./";
            CT.isShow = 0;
            CT.isConfig = 1;
            CT.Parameters = new Dictionary<string, string>();
            CT.Parameters.Add("TMPA", "double|模拟开始时所有物体的温度。默认值是20℃。");
            CT.Parameters.Add("HUMIDITY", "double|相对湿度。单位%。这是在模拟中不只存在火还存在水的情况下需要详细说明的。否则，水的蒸发不能明确的被跟踪。默认值40%。");
            CT.Parameters.Add("GVEC", "double,double,double|重力的三个组成部分，单位m/s2。默认值是GVEC=0,0,-9.81。");
            CT.Parameters.Add("SURF_DEFAULT", "char|设定模拟场景的默认边界材质");
            CT.Memory = "数据文件中只需要一个MISC行";
            CTList.Add(CT);

            CT = new CommandType();
            CT.ID = Guid.Parse("68B2E9F6-61EE-4F8A-B556-AF77470C8D46");
            CT.cmdEnum = CommandEnum.MESH;
            CT.Command = CommandEnum.MESH.ToString();
            CT.CommandNote = "定义计算网格，一个网格是一个正平行六面体,它的数量取决于流动模拟的想得到的辨析率。";
            CT.Demo = "&MESH ID='MESH', IJK=70,20,20, XB=-22.2,-1.2,-3.0,3.0,-1.2,4.8/";
            CT.isShow = 1;
            CT.isConfig = 1;
            CT.Parameters = new Dictionary<string, string>();
            CT.Parameters.Add("ID", "char|设置网格定义的ID");
            CT.Parameters.Add("XB", "double,double,double,double,double,double|网格的分布范围。X1,X2,Y1,Y2,Z1,Z2");
            CT.Parameters.Add("IJK", "double,double,double|定义网格的大小，取决于流动模拟的想得到的辨析率。");
            CT.Memory = "";
            CTList.Add(CT);

            CT = new CommandType();
            CT.ID = Guid.Parse("EA03FB99-8B8A-42EB-A5DC-B5E3649D761E");
            CT.cmdEnum = CommandEnum.REAC;
            CT.Command = CommandEnum.REAC.ToString();
            CT.CommandNote = "定义一个火灾模型（火源），描述其化学性质。";
            CT.Demo = "&REAC ID='POLYURETHANE',FYI = 'NFPA Babrauskas',C = 6.3,H = 7.1,O = 2.1,N = 1.0,CO_YIELD = 0.1,SOOT_YIELD = 0.1 / ";
            CT.isShow = -1;
            CT.isConfig = 1;
            CT.Parameters = new Dictionary<string, string>();
            CT.Parameters.Add("C", "double|燃烧物的碳元素含量");
            CT.Parameters.Add("H", "double|燃烧物的氢元素含量");
            CT.Parameters.Add("O", "double|燃烧物的氧元素含量");
            CT.Parameters.Add("N", "double|燃烧物的氮元素含量");
            CT.Parameters.Add("OTHER", "double|燃烧物的其他元素含量");
            CT.Parameters.Add("CO_YIELD", "double|转化为一氧化物的燃料质量分数，yco。（只是混合分数，默认值0.0）");
            CT.Parameters.Add("SOOT_YIELD", "double|转化为烟微粒的燃料质量分数，ys。注意这个参数不能应用于烟灰的生成和氧化过程，而是从火中产生的烟微粒的静产品。（只是混合分数默认0.01）");
            CT.Parameters.Add("HEAT_OF_COMBUSTION", "double|每消耗一单位的燃料所释放的能量。");
            CT.Parameters.Add("EPUMO2", "double|每消耗一单位质量的氧气所释放出的能量。默认值是13100kj/kg。");
            CT.Memory = "如果EPUMO2和HEAT_OF_COMBUSTION都指定了，那么FDS将忽略EPUMO2的值。";
            CTList.Add(CT);


            CT = new CommandType();
            CT.ID = Guid.Parse("3906F079-A80C-4EB9-A9B2-E6C199FFF2D5");
            CT.cmdEnum = CommandEnum.DEVC;
            CT.Command = CommandEnum.DEVC.ToString();
            CT.CommandNote = "定义模拟场景中的装置";
            CT.Demo = "&DEVC ID='zxT 0M', QUANTITY='THERMOCOUPLE', XYZ=0.0,0.0,1.76/";
            CT.isShow = -1;
            CT.isConfig = 1;
            CT.Parameters = new Dictionary<string, string>();
            CT.Parameters.Add("ID", "char|设置装置定义的ID");
            CT.Parameters.Add("PROP_ID", "char|绑定指定ID的装置性质定义，参见PROP命令");
            CT.Parameters.Add("XYZ", "double,double,double|定义装置的安装位置。");
            CT.Parameters.Add("ORIENTATION", "double,double,double|定义装置的方向。默认值是（0，0，-1）");
            CT.Parameters.Add("IOR", "double|定义装置的方向");
            CT.Parameters.Add("QUANTITY", "char|装置的种类 \r\nTEMPERATURE:温度计\r\nTHERMOCOUPLE:热电偶测温\r\nSPRINKLER LINK TEMPERATURE:洒水装置\r\nVELOCITY:速度监测\r\nMASS FRACTION:质量分数\r\nVISIBILITY:能见度监测\r\nDENSITY:密度监测");
            CT.Memory = "";
            CTList.Add(CT);

            CT = new CommandType();
            CT.ID = Guid.Parse("EF357CD8-E6FB-405D-810B-8F683F19C4C4");
            CT.cmdEnum = CommandEnum.MATL;
            CT.Command = CommandEnum.MATL.ToString();
            CT.CommandNote = "对物理材质的描述";
            CT.Demo = "&MATL ID='不锈钢',SPECIFIC_HEAT = 1.0,CONDUCTIVITY = 16.2,DENSITY = 7780.0 / ";
            CT.isShow = 0;
            CT.isConfig = 1;
            CT.Parameters = new Dictionary<string, string>();
            CT.Parameters.Add("ID", "char|设置材质定义的引用ID");
            CT.Parameters.Add("SPECIFIC_HEAT", "double|液体或固体液滴/微粒的比热。（默认值是4.184kj/kg/K）");
            CT.Parameters.Add("DENSITY", "double|液体或固体液滴/微粒的密度。（默认值1000kg/m3）");
            CT.Parameters.Add("CONDUCTIVITY", "double|液体或固体液滴/微粒的热导率");
            CT.Parameters.Add("HEAT_OF_COMBUSTION", "double|指每单位质量的燃料气体所释放的能量");
            CT.Memory = "这是一个配置项";
            CTList.Add(CT);

            CT = new CommandType();
            CT.ID = Guid.Parse("1D6B3069-63F7-45BF-A81D-810EADCC202A");
            CT.cmdEnum = CommandEnum.SURF;
            CT.Command = CommandEnum.SURF.ToString();
            CT.CommandNote = "定义了所有固体表面的结构或者里边的开口或者流体域的范围。物体和通风孔的边界条件依据合适的SURF行来得到描述";
            CT.Demo = "&SURF ID='不锈钢',COLOR = 'GRAY 80',MATL_ID(1, 1) = '不锈钢',MATL_MASS_FRACTION(1, 1) = 1.0,THICKNESS(1) = 0.01 / ";
            CT.isShow = 0;
            CT.isConfig = 1;
            CT.Parameters = new Dictionary<string, string>();
            CT.Parameters.Add("ID", "char|设置表面边界定义的引用ID");
            CT.Parameters.Add("COLOR", "char|表面颜色");
            CT.Parameters.Add("RGB", "int,int,int|表面颜色");
            CT.Parameters.Add("MATL_ID", "char,char....|表面的材质构成，可以指定为复合材料（1，2）前者表明材质层级，后者表示第几种材质\r\nMATL_ID(1,1:2) = 'BRICK','WATER':在这个例子当中，材料的第一层，Layer1,由砖和水的混合物组成。");
            CT.Parameters.Add("MATL_MASS_FRACTION", "double,double......|表面材质的质量分数 \r\n MATL_MASS_FRACTION(1,1:2) = 0.95,0.05 : 在这个例子和MATL_ID的定义中，砖的质量组成是95%，水是5%");
            CT.Parameters.Add("HRRPUA", "double|指定单位空间热释放率");
            CT.Parameters.Add("THICKNESS", "double|表面材料的厚度");
            CT.Parameters.Add("RAMP_Q", "char|指定表面材料的温度系数");
            CT.Parameters.Add("IGNITION_TEMPERATURE", "double|指定表面材料的燃点温度");
            CT.Parameters.Add("BURN_AWAY", "bool|该物体燃烧后会不会消失\r\nBURN_AWAY=.TURE. 会消失");
            CT.Parameters.Add("TAU_Q", "double|热量释放率\r\n如果TAU_Q是正的，那么热释放率像双曲正切线（t/τ）那样上升。如果是负的，那么HRR像（t/τ）2那样上升。");
            CT.Parameters.Add("TAU_T", "double|表面温度");
            CT.Parameters.Add("TAU_V", "double|燃烧速度 \r\n 和/或正常速度（VEL,VOLUME_FLUX）");
            CT.Memory = "材料层的最大数是20.材料组成的最大数是20.";
            CTList.Add(CT);

            CT = new CommandType();
            CT.ID = Guid.Parse("ADB33575-A981-44FE-A1FD-13502035A11B");
            CT.cmdEnum = CommandEnum.RAMP;
            CT.Command = CommandEnum.RAMP.ToString();
            CT.CommandNote = "指定一个含有一个自变量(例如时间)的函数，映射到一个因变量（例如速度）";
            CT.Demo = "&RAMP ID='BURNER_RAMP_Q', T=3.0, F=0.00105525/";
            CT.isShow = 0;
            CT.isConfig = 1;
            CT.Parameters = new Dictionary<string, string>();
            CT.Parameters.Add("ID", "char|设置函数的引用ID");
            CT.Parameters.Add("T", "double|时间，确定规定的函数从T=0.0开始。");
            CT.Parameters.Add("F", "表示热释放率的分数，墙体温度，速度，质量分数应用等等");
            CT.Memory = "这是一个配置项，注意，每一个RAMP行集合必须有一个相同的ID，并且这些行必须按照无变化的增长的T来列出。";
            CTList.Add(CT);


            CT = new CommandType();
            CT.ID = Guid.Parse("B011549F-1F36-4C17-9B29-C5877E8DFC6E");
            CT.cmdEnum = CommandEnum.OBST;
            CT.Command = CommandEnum.OBST.ToString();
            CT.CommandNote = "在场景中指定一个物体（障碍物）";
            CT.Demo = "&OBST XB=0.0,20.0,-0.95,0.95,2.15,2.25, RGB=51,51,255, SURF_ID='铝蜂窝 8mm'/ aluminium honeycomb";
            CT.isShow = 1;
            CT.isConfig = 1;
            CT.Parameters = new Dictionary<string, string>();
            CT.Parameters.Add("XB", "double,double,double,double,double,double|物体的空间范围。X1,X2,Y1,Y2,Z1,Z2");
            CT.Parameters.Add("COLOR", "char|表面颜色");
            CT.Parameters.Add("RGB", "int,int,int|表面颜色");
            CT.Parameters.Add("PERMIT_HOLE", "bool|物体表面是否能被穿孔");
            CT.Parameters.Add("TRANSPARENCY", "double|0-1的透明度");
            CT.Memory = "还有很多参数，似乎没用到";
            CTList.Add(CT);

            CT = new CommandType();
            CT.ID = Guid.Parse("D619EFA8-D066-4DAC-870E-0677D13B0B50");
            CT.cmdEnum = CommandEnum.HOLE;
            CT.Command = CommandEnum.HOLE.ToString();
            CT.CommandNote = "在现有的物体（障碍物或一组障碍物）上切开一个空间";
            CT.Demo = "&OBST XB=1.0,1.1,0.0,5.0,0.0,3.0 /";
            CT.isShow = 1;
            CT.isConfig = 1;
            CT.Parameters = new Dictionary<string, string>();
            CT.Parameters.Add("XB", "double,double,double,double,double,double|孔洞的空间范围。X1,X2,Y1,Y2,Z1,Z2");
            CT.Parameters.Add("COLOR", "char|孔洞颜色");
            CT.Parameters.Add("RGB", "int,int,int|孔洞颜色");
            CT.Parameters.Add("TRANSPARENCY", "double|0-1的透明度");
            CT.Memory = "注意HOLE对VENT或者网格边界没有任何的影响。它只应用于OBSTstructions。";
            CTList.Add(CT);

            CT = new CommandType();
            CT.ID = Guid.Parse("546B8DDB-4A32-45BA-A612-50E40C5BB821");
            CT.cmdEnum = CommandEnum.VENT;
            CT.Command = CommandEnum.VENT.ToString();
            CT.CommandNote = "用来指定毗邻物体（障碍物或外墙）的平面";
            CT.Demo = "&VENT SURF_ID='INERT', XB=-22.2,-1.2,3.0,3.0,-1.2,4.8/ Mesh Vent: MESH [YMAX]";
            CT.isShow = 1;
            CT.isConfig = 1;
            CT.Parameters = new Dictionary<string, string>();
            CT.Parameters.Add("XB", "double,double,double,double,double,double|表面的空间范围。X1,X2,Y1,Y2,Z1,Z2");
            CT.Parameters.Add("SURF_ID", "char|表面材质的描述");
            CT.Parameters.Add("COLOR", "char|平面颜色");
            CT.Parameters.Add("RGB", "int,int,int|平面颜色");
            CT.Parameters.Add("IOR", "double|平面的方向");
            CT.Memory = "注意对于任何给定的墙体单元只能指定一个VENT。如果对一个给定的墙体单元指定额外的VENT行，FDS将会产生一个警告信息，并且忽略后续的行。（也就是说，只应用到了第一个vent）";
            CTList.Add(CT);


            CT = new CommandType();
            CT.ID = Guid.Parse("DEBE9A82-1809-458F-B74E-97F653F92B7C");
            CT.cmdEnum = CommandEnum.SLCF;
            CT.Command = CommandEnum.SLCF.ToString();
            CT.CommandNote = "SLCF名称组参数使你能够在多于一个单独的点处记录各种的气相量";
            CT.Demo = "&SLCF QUANTITY='MASS FRACTION', SPEC_ID='carbon monoxide', PBY=0.0/";
            CT.isShow = 1;
            CT.isConfig = 1;
            CT.Parameters = new Dictionary<string, string>();
            CT.Parameters.Add("XB", "double,double,double,double,double,double|表面的空间范围。X1,X2,Y1,Y2,Z1,Z2");
            CT.Parameters.Add("SURF_ID", "char|表面材质的描述");
            CT.Parameters.Add("QUANTITY", "char|监测的种类 \r\nTEMPERATURE: 温度计\r\nTHERMOCOUPLE: 热电偶测温\r\nSPRINKLER LINK TEMPERATURE: 洒水装置\r\nVELOCITY: 速度监测\r\nMASS FRACTION: 质量分数\r\nVISIBILITY: 能见度监测\r\nDENSITY: 密度监测");
            CT.Parameters.Add("PBX", "double|平面基准坐标");
            CT.Parameters.Add("PBY", "double|平面基准坐标");
            CT.Parameters.Add("PBZ", "double|平面基准坐标");
            CT.Parameters.Add("SPEC_ID", "double|平面的方向");
            CT.Memory = "";
            CTList.Add(CT);

            CT = new CommandType();
            CT.ID = Guid.Parse("D492B60C-EC75-4728-8BED-28F9A7FD69F7");
            CT.cmdEnum = CommandEnum.SPEC;
            CT.Command = CommandEnum.SPEC.ToString();
            CT.CommandNote = "额外气体类别";
            CT.Demo = "&SPEC ID='ARGON',MASS_FRACTION_0=0.1,MW=40. /";
            CT.isShow = 0;
            CT.isConfig = 1;
            CT.Parameters = new Dictionary<string, string>();
            CT.Parameters.Add("ID", "char|设置气体的引用ID");
            CT.Parameters.Add("MASS_FRACTION_0", "double|如果气体的环境（最初）质量分数不是0，那么用参数MASS_FRACTION_0来指定它。");
            CT.Parameters.Add("MW", "double|指定气体的摩尔质量");
            CT.Parameters.Add("PBX", "double|平面基准坐标");
            CT.Parameters.Add("PBY", "double|平面基准坐标");
            CT.Parameters.Add("PBZ", "double|平面基准坐标");
            CT.Parameters.Add("SPEC_ID", "double|平面的方向");
            CT.Memory = "该配置有一部分FDS内置的ID，通常可以使用FDS自定义的值。例如SOOT，carbon monoxide等";
            CTList.Add(CT);

            CT = new CommandType();
            CT.ID = Guid.Parse("D492B60C-EC75-4728-8BED-28F9A7FD69F7");
            CT.cmdEnum = CommandEnum.TAIL;
            CT.Command = CommandEnum.TAIL.ToString();
            CT.CommandNote = "FDS文件结束标志";
            CT.Demo = "&TAIL /";
            CT.isShow = 0;
            CT.isConfig = 1;
            CT.Parameters = new Dictionary<string, string>();
            CT.Memory = "";
            CTList.Add(CT);

            System.Web.Script.Serialization.JavaScriptSerializer Serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            Note = Serializer.Serialize(CTList);
            
            return Note;
        }

        public enum TypeEnum
        {
            JSON=0,
            XML=1,
        }

        public enum CommandEnum
        {
            OTHER= -1,
            NULL = 0,
            HEAD = 1,
            TIME = 2,
            DUMP = 3,
            MISC = 4,
            MESH = 5,
            REAC = 6,
            DEVC = 7,
            MATL = 8,
            SURF = 9,
            RAMP = 10,
            OBST = 11,
            HOLE = 12,
            VENT = 13,
            SLCF = 14,
            SPEC = 15,
            TAIL = 16,
        }

        public struct struData
        {
            public List< double[]> pos;
            public Dictionary<string,string> mat;
        }

        public struct struInfo
        {
            public string code;
            public List<struData> objects;
            public List<struData> hole;
        }

        public struct stru3DObj
        {
            public string id;
            public string name;
            public List<struInfo> components;
        }


        public static string Get3DObject(string Data,string Type = "json")
        {
            string[] info = Data.Split('\n');
            Data = string.Empty;
            return Get3DObject(info);
        }

        public static string Get3DObject()
        {
            return Get3DObject(Reader());
        }


        public static string Get3DObject(string[] Data)
        {
            MinX = double.MaxValue; MinY = double.MaxValue; MinZ = double.MaxValue; MaxX = double.MinValue; MaxY = double.MinValue; MaxZ = double.MinValue;

            string OrgLine = string.Empty;
            struCommand[] cmdList = ProcCommand(Data);
            Data = null;

            stru3DObj Obj3D = new stru3DObj();
            Obj3D.id = "style1";
            Obj3D.name = "座椅1";
            Obj3D.components = new List<struInfo>();

            foreach (string data in NoteList)
            {
                struInfo SI = new struInfo();
                SI.code = data;
                SI.objects = new List<struData>();
                SI.hole = new List<struData>();
                Obj3D.components.Add(SI);
            }

            for(int i =0;i<cmdList.Length;i++)
            {
                struCommand cmd = cmdList[i];
                string command = cmd.strCommand.Replace("&", "").Trim().ToUpper();
                if (cmd.cmdEnum == CommandEnum.OBST || cmd.cmdEnum ==CommandEnum.HOLE)
                {
                    int NoteIndex = 0;
                    for(int j=0;j< Obj3D.components.Count;j++)
                    {
                        if(Obj3D.components[j].code==cmd.Note.Trim())
                        {
                            NoteIndex = j;
                            break;
                        }
                    }
                    string value = string.Empty;
                    cmd.Parameters.TryGetValue("XB", out value);

                    struData data = new struData();
                    data.mat = new Dictionary<string, string>();
                    data.mat.Add("c", cmd.c);

                    if (!string.IsNullOrEmpty(value))
                    {
                            Point P1 = cmd.p1;
                            Point P2 = cmd.p2;

                            double[] t = new double[3];
                            t[0] = Convert.ToDouble((P1.X - MinX).ToString("N6"));
                            t[1] = Convert.ToDouble((P1.Y - MinY).ToString("N6"));
                            t[2] = Convert.ToDouble((P1.Z - MinZ).ToString("N6"));
                            double[] t1 = new double[3];
                            t1[0] = Convert.ToDouble((P2.X - MinX).ToString("N6"));
                            t1[1] = Convert.ToDouble((P2.Y - MinY).ToString("N6"));
                            t1[2] = Convert.ToDouble((P2.Z - MinZ).ToString("N6"));


                        data.pos = new List<double[]>();
                            data.pos.Add(t);
                            data.pos.Add(t1);
                        if (cmd.cmdEnum == CommandEnum.OBST)
                        {
                            Obj3D.components[NoteIndex].objects.Add(data);
                        }
                        if (cmd.cmdEnum == CommandEnum.HOLE)
                        {
                            Obj3D.components[0].hole.Add(data);
                        }
                        //Obj3D.components.Add( SI);

                        {
                            OrgLine += string.Format("&OBST XB={0},{1},{2},{3},{4},{5}/\r\n",
                                    P1.X.ToString(),
                                    P2.X.ToString(),
                                    P1.Y.ToString(),
                                    P2.Y.ToString(),
                                    P1.Z.ToString(),
                                    P2.Z.ToString());
                        }
                    }
                }
            }
            Console.WriteLine("MinX:" + MinX.ToString());
            Console.WriteLine("MaxX:" + MaxX.ToString());
            Console.WriteLine("MinY:" + MinY.ToString());
            Console.WriteLine("MaxY:" + MaxY.ToString());
            Console.WriteLine("MinZ:" + MinZ.ToString());
            Console.WriteLine("MaxZ:" + MaxZ.ToString());

            System.Web.Script.Serialization.JavaScriptSerializer Serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string o = Serializer.Serialize(Obj3D);

            System.IO.File.AppendAllText(@"C:\Data\info.json", o);
            //System.IO.File.AppendAllText(@"C:\Data\info.fds", OrgLine);
            cmdList = null;
            GC.Collect();
                return o;
        }

        public struct Point
        {
            public double X;
            public double Y;
            public double Z;
        }

        public static List<string> NoteList = new List<string>();

        static void AddNoteList(string Note)
        {
            Note = Note.Trim();
            bool InList = false;
            foreach(string data in NoteList)
            {
                if(data==Note)
                {
                    InList = true;
                    break;
                }
            }
            if(!InList)
            {
                NoteList.Add(Note);
            }
        }

        static CommandEnum GetCommandEnum(string strCommand)
        {
            strCommand = strCommand.Replace("&", "").Trim().ToUpper();

            if (string.IsNullOrEmpty(strCommand)) return CommandEnum.NULL;
            try
            {
                return (CommandEnum)Enum.Parse(typeof(CommandEnum), strCommand);
            }
            catch
            {
                return CommandEnum.OTHER;
            }
        }

        public static struCommand[] ProcCommand(string[] data)
        {//整理命令
            NoteList.Clear();
            NoteList.Add("other");
            List<struCommand> CmdList = new List<struCommand>();
            for (int i = 0; i < data.Length; i++)
            {
                string Cmd = data[i].Trim();
                if (string.IsNullOrEmpty(Cmd)) continue;
                if (Cmd.Substring(0, 1) == "&")
                {
                    struCommand Command = new struCommand();
                    string comm = Cmd.Substring(0, Cmd.IndexOf(" ")).Trim();

                    Command.strCommand = comm;
                    Command.cmdEnum = GetCommandEnum(comm);
                    Command.ID = Guid.NewGuid();
                    Command.LineNUM = i;
                    Command.Data = Cmd;
                    Command.Parameters = new Dictionary<string, string>();
                    string tmp = Cmd.Substring(Cmd.IndexOf(" "), Cmd.Length - Cmd.IndexOf(" ")).Trim();
                    //Console.WriteLine(tmp);
                    string Key = string.Empty;
                    string Value = string.Empty;

                    string[] NameValue = tmp.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string temp in NameValue)
                    {
                        if (string.IsNullOrEmpty(temp.Trim())) continue;
                        string[] info = temp.Trim().Split('=');
                        if (info.Length != 2) continue;

                        if (info[1].IndexOf('/') > 0)
                        {
                            string[] p = info[1].Split('/');
                            info[1] = p[0];
                            Command.Note = p[1];
                            AddNoteList(p[1]);
                        }

                        if(info[0]=="ID")
                        {
                            Command.CMDID = info[1];
                        }

                        Command.Parameters.Add(info[0], info[1]);

                    }
                    CmdList.Add(Command);
                }
            }

            struCommand[] cmdList = CmdList.ToArray();
            CmdList = null;
            for (int i = 0; i < cmdList.Length; i++)
            {

                string temp = string.Empty;
                if (cmdList[i].Parameters.TryGetValue("RGB", out temp))
                {
                    string rgb = GetColor("RGB", temp);
                    if (!string.IsNullOrEmpty(rgb)) cmdList[i].c = rgb;
                }
                if (cmdList[i].Parameters.TryGetValue("COLOR", out temp))
                {
                    string rgb = GetColor("COLOR", temp);
                    if (!string.IsNullOrEmpty(rgb)) cmdList[i].c = rgb;
                }
                if (cmdList[i].Parameters.TryGetValue("SURF_ID", out temp))
                {
                    string rgb = GetColor("SURF_ID", temp, cmdList);
                    if (!string.IsNullOrEmpty(rgb)) cmdList[i].c = rgb;
                }

                temp = string.Empty;
                if (cmdList[i].Parameters.TryGetValue("XB", out temp))
                {
                    string[] valuelist = temp.Split(',');
                    if (valuelist.Length >= 6)
                    {
                        cmdList[i].p1 = new Point();
                        cmdList[i].p2 = new Point();

                        cmdList[i].p1.X = Convert.ToDouble(valuelist[0]);
                        cmdList[i].p2.X = Convert.ToDouble(valuelist[1]);

                        cmdList[i].p1.Y = Convert.ToDouble(valuelist[2]);
                        cmdList[i].p2.Y = Convert.ToDouble(valuelist[3]);

                        cmdList[i].p1.Z = Convert.ToDouble(valuelist[4]);
                        cmdList[i].p2.Z = Convert.ToDouble(valuelist[5]);

                        if (cmdList[i].p1.X < MinX) MinX = cmdList[i].p1.X;
                        if (cmdList[i].p1.X > MaxX) MaxX = cmdList[i].p1.X;

                        if (cmdList[i].p1.Y < MinY) MinY = cmdList[i].p1.Y;
                        if (cmdList[i].p1.Y > MaxY) MaxY = cmdList[i].p1.Y;

                        if (cmdList[i].p1.Z < MinZ) MinZ = cmdList[i].p1.Z;
                        if (cmdList[i].p1.Z > MaxZ) MaxZ = cmdList[i].p1.Z;

                        if (cmdList[i].p2.X < MinX) MinX = cmdList[i].p2.X;
                        if (cmdList[i].p2.X > MaxX) MaxX = cmdList[i].p2.X;

                        if (cmdList[i].p2.Y < MinY) MinY = cmdList[i].p2.Y;
                        if (cmdList[i].p2.Y > MaxY) MaxY = cmdList[i].p2.Y;

                        if (cmdList[i].p2.Z < MinZ) MinZ = cmdList[i].p2.Z;
                        if (cmdList[i].p2.Z > MaxZ) MaxZ = cmdList[i].p2.Z;
                    }
                }
            }
            data = null;
            GC.Collect();
            return cmdList;
        }

        static string GetColor(string Key,string Value , struCommand[] list =null )
        {
            string Color = string.Empty;

            if(Key== "SURF_ID")
            {
                foreach(struCommand cmd in list)
                {
                    if (string.IsNullOrEmpty(cmd.CMDID)) continue;
                    if(cmd.cmdEnum ==CommandEnum.SURF)
                    {
                        if (cmd.CMDID.Replace("'", "").Replace(",","")==Value.Replace("'", ""))
                        {
                            string temp = string.Empty;
                            if (cmd.Parameters.TryGetValue("RGB", out temp))
                            {
                                if (!string.IsNullOrEmpty(temp))
                                {
                                    Key = "RGB";
                                    Value = temp;
                                    break;
                                }
                            }
                            temp = string.Empty;
                            if (cmd.Parameters.TryGetValue("COLOR", out temp))
                            {
                                if (!string.IsNullOrEmpty(temp))
                                {
                                    Key = "COLOR";
                                    Value = temp;
                                    break;
                                }
                            }

                        }
                    }
                }

            }


            if(Key=="RGB")
            {
                string[] str = Value.Split(',');
                Color = Convert.ToInt32(str[0]).ToString("X2");
                Color += Convert.ToInt32(str[1]).ToString("X2");
                Color += Convert.ToInt32(str[2]).ToString("X2");
            }
            if(Key =="COLOR")
            {
                Value = Value.Replace("'", "");
                Value = Value.Replace("\"", "");
                if (Value == "RED") Color = "FF0000";
                if (Value == "GREEN") Color = "00FF00";
                if (Value == "BLUE") Color = "0000FF";
                if (Value == "BLACK") Color = "000000";
                if (Value == "BLACK") Color = "000000";
                if (Value == "GRAY 80") Color = "808080";
                if (Value == "GRAY 60") Color = "606060";
                if (Color == string.Empty) Console.WriteLine("Unknow Color:" + Value);
            }


            return Color;
        }

        public static string[] PreProcCommand(string[] infoList)
        {//预处理，整合成单行命令

            List<string> CommandStringList = new List<string>();
            string CommandLine = string.Empty;

            foreach (string command in infoList)
            {
                string info = command.Trim();
                if (string.IsNullOrEmpty(info)) continue;
                if (info.Substring(0, 1) == "&")
                {
                    CommandLine = info;
                }
                else
                {
                    CommandLine += " " + info;
                }
                if (CommandLine.IndexOf("/") > 0)
                {
                    CommandStringList.Add(CommandLine);
                    //Console.WriteLine(CommandLine);
                    CommandLine = string.Empty;
                }
            }
            return CommandStringList.ToArray();
        }



        public static string[] GetCommandList(string[] infoList)
        {
            List<string> CommandList = new List<string>();


            foreach (string command in infoList)
            {
                string info = command.Trim();
                if (string.IsNullOrEmpty(info)) continue;
                if (info.Substring(0, 1) == "&")
                {
                    string comm = info.Substring(0, info.IndexOf(" "));
                    bool inList = false;
                    foreach (string t in CommandList)
                    {
                        if (t == comm) inList = true;
                    }
                    if (!inList)
                    {
                        CommandList.Add(comm);
                    }
                }

            }

            return CommandList.ToArray();
        }

    }
}
