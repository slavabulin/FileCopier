using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCopier
{
    [Serializable]
    public class FileData
    {
        public Path[] paths;

        public class Path
        {
            string pathFrom;
            string pathTo;
            string fileName;
            public string FileName
            {
                get
                {
                    return fileName;
                }
                set
                {
                    this.fileName = value;
                }
            }
            public string From
            {
                get
                {
                    return pathFrom;
                }
                set
                {
                    this.pathFrom = value;
                }
            }
            public string To
            {
                get
                {
                    return pathTo;
                }
                set
                {
                    this.pathTo = value;
                }
            }

        }

    }
}
