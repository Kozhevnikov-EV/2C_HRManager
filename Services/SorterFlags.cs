using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;

namespace Homework_12_ver_1
{
    /// <summary>
    /// Статичный класс с набором флагов сортировки. Каждый флаг меняется в зависимости от того, какая последняя сортировка была выполнена
    /// (по возрастанию или убыванию). Каждому свойству класса Worker присвоен свой флаг.
    /// </summary>
    public static class SorterFlags
    {
        static SorterFlags()
        {
            idFlag = true;
            nameFlag = true;
            surnameFlag = true;
            ageFlag = true;
            positionFlag = true;
            salaryFlag = true;
            projectsFlag = true;
        }

        private static bool idFlag;

        public static bool IdFlag
        {
            get
            {
                if (idFlag)
                {
                    idFlag = false;
                    return true;
                }
                else
                {
                    idFlag = true;
                    return false;
                }
            }
        }

        private static bool nameFlag;

        public static bool NameFlag
        {
            get
            {
                if (nameFlag)
                {
                    nameFlag = false;
                    return true; 
                }
                else
                {
                    nameFlag = true;
                    return false;
                }
            }
        }

        private static bool surnameFlag;

        public static bool SurnameFlag
        {
            get
            {
                if (surnameFlag)
                {
                    surnameFlag = false;
                    return true;
                }
                else
                {
                    surnameFlag = true;
                    return false;
                }
            }
        }

        private static bool ageFlag;

        public static bool AgeFlag
        {
            get
            {
                if (ageFlag)
                {
                    ageFlag = false;
                    return true;
                }
                else
                {
                    ageFlag = true;
                    return false;
                }
            }
        }

        private static bool positionFlag;

        public static bool PositionFlag
        {
            get
            {
                if (positionFlag)
                {
                    positionFlag = false;
                    return true;
                }
                else
                {
                    positionFlag = true;
                    return false;
                }
            }
        }

        private static bool salaryFlag;

        public static bool SalaryFlag
        {
            get
            {
                if (salaryFlag)
                {
                    salaryFlag = false;
                    return true;
                }
                else
                {
                    salaryFlag = true;
                    return false;
                }
            }
        }

        private static bool projectsFlag;

        public static bool ProjectsDescending
        {
            get
            {
                if (projectsFlag)
                {
                    projectsFlag = false;
                    return true;
                }
                else
                {
                    projectsFlag = true;
                    return false;
                }
            }
        }


    }
}
