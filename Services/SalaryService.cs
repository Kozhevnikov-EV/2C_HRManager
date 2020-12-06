using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Homework_12_ver_1
{
    /// <summary>
    /// Статический класс-сервис для рассчета заработной платы руководителей департаментов и зарплатных фондов департаментов
    /// </summary>
    static class SalaryService
    {
        /// <summary>
        /// Основной метод расчета ЗП руководителей и зарплатного фонда департаментов
        /// </summary>
        public static void MainMethodSetSalaryForManagers(Organization organization)
        {
            if (organization.Departments.Count != 0)
            {
                ResetSalaryFlags(organization); //сбрасываем флаги о проведении расчета ЗП
                SetLowerManagersSalary(organization); //метод расчета ЗП руководителей и зарп.фонда департаментов низших уровней
                                                      //(без подчиненных департаментов)
                SetHigherManagersSalary(organization);  //метод расчета ЗП вышестоящих руководителей и зарплатного фонда вышестоящих департаментов
            }
        }

        /// <summary>
        /// Метод сброса флагов департаментов, указывающих о проведении расчета ЗП руководителя и ЗП-фонда
        /// </summary>
        /// <param name="organization">организация</param>
        private static void ResetSalaryFlags(Organization organization)
        {
            foreach (var e in organization.Departments) //в цикле сбрасываем флаги
            {
                e.SalaryCalculatedFlag = false;
            }
        }

        /// <summary>
        /// Расчет ЗП руководителей и зарплатных фондов департаментов низших уровней (без подчиненных сотрудников)
        /// </summary>
        /// <param name="organization">организация</param>
        private static void SetLowerManagersSalary(Organization organization)
        {
            int Id; //переменная для хранения Id текущего руководителя
            int SumOfSalary = 0; //сюда будем записывать текущее значение ЗП;
            for (int i = 0; i < organization.Departments.Count; i++) //цикл, перебирающий департаменты в коллекции департаментов
            {
                if (organization.Departments[i].SlaveDepartmentId.Count == 0) //если у департамента нет других департаментов в подчинении
                {
                    Id = organization.Departments[i].ChiefId; //в переменную записываем значение Id руководителя этого департамента
                    SumOfSalary += FindEmployeesSalary(organization, organization.Departments[i].Id); // считаем сумму ЗП работников данного департамента
                    organization.Departments[i].SalaryFund = 0; //обнуляем текущее значение зарплатного фонда
                    if (Id != 0)
                    {
                        foreach (var worker in organization.Workers) //цикл, перебирающий работников в коллекции работников
                        {
                            if (worker.Id == Id) //находим Начальника департамента
                            {
                                //расчитываем ему ЗП и записываем в соответствующее поле
                                worker.Salary = SumOfSalary * Manager.Procent / 100; 
                                organization.Departments[i].SalaryFund += worker.Salary; //в зарплатный фонд добавляем ЗП руководителя

                            }
                        }
                    }
                    organization.Departments[i].SalaryFund += SumOfSalary; //в зарплатный фонд добавляем ЗП сотрудников
                    organization.Departments[i].SalaryCalculatedFlag = true; //Делаем отметку в департаменте, что ЗП расчитана
                    SumOfSalary = 0; //обнуляем переменную, хранящую текущее значение ЗП
                }
            }
        }

        /// <summary>
        /// Расчет ЗП руководителей и зарплатных фондов вышестоящих департаментов
        /// </summary>
        /// <param name="organization">организация</param>
        private static void SetHigherManagersSalary(Organization organization)
        {
            while (!AllSalaryIsKnown(organization)) //метод выполняется до тех пор, пока не рассчитаем все зарплатные фонды всех департаментов
            {
                int SumOfSalary = 0; //сюда будем записывать текущее значение ЗП;
                for (int i = 0; i < organization.Departments.Count; i++) //в цикле перебираем все департаменты в коллекции департаментов
                {
                    //Если ЗП фонд департамента не расчитан, а ЗП фонд подчиненных ему департаментов рассчитан, то
                    if (organization.Departments[i].SalaryCalculatedFlag == false 
                        && ReadyForSalaryCalculation(organization, organization.Departments[i].SlaveDepartmentId) == true)
                    {
                        organization.Departments[i].SalaryFund = 0; //сбрасываем значение ЗП фонда департамента
                        //добавляем сумму ЗП сотрудников департаментов
                        SumOfSalary += FindEmployeesSalary(organization, organization.Departments[i].Id);
                        organization.Departments[i].SalaryFund += SumOfSalary;
                        //суммируем зарплатные фонды нижестоящих подчиненных департаментов
                        SumOfSalary += SalaryFundLowerDepartments(organization , organization.Departments[i].SlaveDepartmentId);

                        if (organization.Departments[i].ChiefId != 0)
                        {
                            foreach (var worker in organization.Workers) //перебираем всех работников в коллекции работников
                            {
                                if (worker.Id == organization.Departments[i].ChiefId) //находим начальника департамента
                                {
                                    worker.Salary = SumOfSalary * Manager.Procent / 100; //считаем ЗП начальника
                                    organization.Departments[i].SalaryFund += worker.Salary; //считаем ЗП фонд департамента
                                }
                            }
                        }
                        organization.Departments[i].SalaryCalculatedFlag = true; //Делаем отметку в департаменте, что ЗП расчитана
                        SumOfSalary = 0; //сбрасываем значение переменной-суммы ЗП
                    }
                }
            }
        }

        /// <summary>
        /// Проверка полноты расчета зарплатных фондов всех департаментов
        /// </summary>
        /// <param name="organization">организация</param>
        /// <returns>true - во всех департаментов рассчитаны зарплатные фонды и ЗП руководителей</returns>
        private static bool AllSalaryIsKnown(Organization organization)
        {
            bool Answer = false; //переменная, значение которой будем возвращать
            foreach (var dep in organization.Departments) //перебираем департаменты в коллекции департаментов
            {
                if (dep.SalaryCalculatedFlag == true) //если ЗП расчитана, то Answer присваиваем true
                {
                    Answer = true;
                }
                else
                {
                    Answer = false; //если хоть в одном департаменте ЗП не расчитана, то прерываем цикл и возвращаем false
                    break;
                }
            }
            return Answer;
        }

        /// <summary>
        /// Расчет суммы зарплатных фондов всех нижестоящих департаментов (рекурсивный метод!)
        /// </summary>
        /// <param name="organization">организация</param>
        /// <param name="DepartmentsIds">Коллекция Id департаментов прямой подчиненности</param>
        /// <returns>Сумму зарплатных фондов всех нижестоящих департаментов</returns>
        private static int SalaryFundLowerDepartments(Organization organization, List<int> DepartmentsIds)
        {
            int SalaryFund = 0; //переменная, куда будем суммировать зарплатные фонды
            foreach (var department in organization.Departments) //перебираем в цикле все департаменты в коллекции департаментов
            {
                if (DepartmentsIds.Contains(department.Id) && department.SalaryCalculatedFlag) //если Id департамента есть в списке прямой подчиненности старшего департамента
                {
                    SalaryFund += department.SalaryFund; //то добавляем его зарплатный фонд в нашу переменную
                    if (department.SlaveDepartmentId.Count != 0) //если у этого департамента также есть в подчинении нижестоящие департаменты
                    {
                        SalaryFund += SalaryFundLowerDepartments(organization, department.SlaveDepartmentId); //то используем тот же метод для поисках их 
                        //запрплатных фондов (рекурсия!)
                    }
                }
            }
            return SalaryFund; //возвращаем сумму зарплатных фондов всех нижестоящих департаментов
        }

        /// <summary>
        /// Проверка наличия рассчитанного зарплатного фонда в подчиненных департаментах
        /// </summary>
        /// <param name="organization">организация</param>
        /// <param name="SlaveDepartmentId">Коллекция Id под подчиненных департаментов</param>
        /// <returns>true - в подчиненных департаментах зарплатные фонды расчитаны</returns>
        private static bool ReadyForSalaryCalculation(Organization organization, List<int> SlaveDepartmentId)
        {
            bool Ready = false; //переменная для возврата из метода
            for (int i = 0; i < organization.Departments.Count; i++) //перебираем все департаменты в коллекции департаментов
            {
                //если в любом из подчиненных департаментов зарплатный фонд не рассчитан
                if (SlaveDepartmentId.Contains(organization.Departments[i].Id) && organization.Departments[i].SalaryCalculatedFlag == false)
                {
                    Ready = false; //то в переменную с ответом пишем false
                    break; //прерываем цикл
                }
                else
                {
                    Ready = true;
                }
            }
            return Ready; //возращаем ответ
        }

        /// <summary>
        /// Метод калькуляции суммы зарплат работников департамента (кроме руководителя)
        /// </summary>
        /// <param name="organization">организация</param>
        /// <param name="DepartmentId">Id департамента</param>
        /// <returns>Возвращает сумму зарплат работников департамента (за исключением руководителя)</returns>
        private static int FindEmployeesSalary(Organization organization, int DepartmentId)
        {
            int WorkersSalarySum = 0; //переменная в которую будем суммировать зп сотрудников
            for (int i = 0; i < organization.Workers.Count; i++) //в цикле перебираем все сотрудников
            {
                //если Id сотрудника совпадает с Id департамента и сотрудник не является экземпляром класса Manager
                if (organization.Workers[i].DepartmentId == DepartmentId && !(organization.Workers[i] is Manager))
                {
                    WorkersSalarySum += organization.Workers[i].Salary; //то зп данного сотрудника добавляем в сумму
                }
            }
            return WorkersSalarySum; //возвращаем сумму зп сотрудников отдела

        }
    }
}
