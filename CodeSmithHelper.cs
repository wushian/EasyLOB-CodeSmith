using CodeSmith.Engine;
using SchemaExplorer;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;

namespace XCodeSmith
{
    public enum Archetypes
    {
        Application,
        Persistence
    }

    public enum Cultures
    {
        en_US, // English
        pt_BR // Brazilian Portuguese
    }
    
    public class CodeSmithHelper : CodeTemplate
    {   

        #region Properties
        
        // Acronyms that should not be renamed in Classes, Objects and Properties Names
        // Array.IndexOf(Acronyms, name) < 0
        private string[] Acronyms
        {
            get
            {
                return new string[]
                {
                    // en-US
                    "ERP",          // Enterprise Resource Planning
                    "CRM",          // Customer Relationship Management
                    "ZIP",          // Zone Improvement Plan
                    // pt-BR
                    "CEP",          // Código de Endereçamento Postal
                    "CFOP",         // Código Fiscal de Operação e Prestação
                    "CNPJ",         // Cadastro Nacional de Pessoas Jurídicas    
                    "CPF",          // Cadastro de Pessoas Físicas
                    "CST",          // Código de Situação Tributária
                    "DDD",          // Discagem Direta a Distância
                    "ICMS",         // Imposto sobre Circulação de Mercadorias e Serviços
                    "ICMSST",       // ICMS Substituição Tributária
                    "IPI",          // Imposto de Produtos Industralizados
                    "IR",           // Imposto de Renda
                    "IRRF",         // Imposto de Renda Retido na Fonte
                    "ISS",          // Imposto sobre Serviços
                    "NCM",          // Nomeclatura Comum do Mercosul
                    "NF",           // Nota Fiscal
                    "UF"            // Unidade da Federação
                };
            }
        }
        
        // Default output path
        private string DefaultOutput = "C:/CodeSmith";
        
        // Rename Classes and Properties using PascalCase and camelCase conventions ?
        // true  :: Customer => Customer | customer => Customer
        // false :: Customer => Customer | customer => customer 
        private bool IsCase { get { return true; } }
        
        #endregion
        
        #region Methods

        public void CreateDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);            
            }
        }

        public static bool IsNullOrEmpty(string s) // 2.6
        {
            return (s == null || s.Trim() == "");
        }
        
        public string Plural(string s, Cultures culture)
        {
            if (culture == Cultures.pt_BR)
            {
                return Plural_pt_BR(s);
            }
            else
            {
                return Plural_en_US(s);
            }
        }

        public string Plural_en_US(string s)        
        {
            string result = "";
            
            s = s.Trim();
            
            if (s.EndsWith("y"))
            {
                result = System.Text.RegularExpressions.Regex.Replace(s, "y$", "ies"); // y => ies
            }
            else if (s.EndsWith("ss"))
            {
                result = System.Text.RegularExpressions.Regex.Replace(s, "ss$", "sses"); // ss => sses
            }
            else if (s.EndsWith("s"))
            {
                result = s;
            }
            else
            {
                result = s + "s"; // ? => s
            }

            return result;
        }

        public string Plural_pt_BR(string s)
        {
            string result = "";
            
            s = s.Trim();
            
            if (s.EndsWith("ao"))
            {
                result = System.Text.RegularExpressions.Regex.Replace(s, "ao$", "oes"); // ao => oes
            }
            else if (s.EndsWith("l"))
            {
                result = System.Text.RegularExpressions.Regex.Replace(s, "l$", "is"); // l => is
            }
            else if (s.EndsWith("m"))
            {
                result = System.Text.RegularExpressions.Regex.Replace(s, "m$", "ns"); // m => ns
            }
            else if (s.EndsWith("r"))
            {
                result = System.Text.RegularExpressions.Regex.Replace(s, "r$", "res"); // r => res
            }
            else
            {
                result = s + "s";
            }
            
            return result;
        }

        public string Singular(string s, Cultures culture)
        {
            if (culture == Cultures.pt_BR)
            {
                return Singular_pt_BR(s);
            }
            else
            {
                return Singular_en_US(s);
            }
        }

        public string Singular_en_US(string s)        
        {
            string result = "";
            
            s = s.Trim();
           
            if (s.EndsWith("ss"))
            {
                result = s;
            }
            else if (s.EndsWith("ies"))
            {
                result = System.Text.RegularExpressions.Regex.Replace(s, "ies$", "y"); // y => ies
            }
            else if (s.EndsWith("sses"))
            {
                result = System.Text.RegularExpressions.Regex.Replace(s, "sses$", "ss"); // ss => sses
            }
            else if (s.EndsWith("s"))
            {
                result = System.Text.RegularExpressions.Regex.Replace(s, "s$", ""); // ? => s
            }
            else
            {
                result = s;
            }

            return result;
        }

        public string Singular_pt_BR(string s)
        {
            string result = "";
            
            s = s.Trim();
            
            if (s.EndsWith("oes"))
            {
                result = System.Text.RegularExpressions.Regex.Replace(s, "oes$", "ao"); // ao => oes
            }
            else if (s.EndsWith("is"))
            {
                result = System.Text.RegularExpressions.Regex.Replace(s, "is$", "l"); // l => is
            }
            else if (s.EndsWith("ns"))
            {
                result = System.Text.RegularExpressions.Regex.Replace(s, "ns$", "m"); // m => ns
            }
            else if (s.EndsWith("res"))
            {
                result = System.Text.RegularExpressions.Regex.Replace(s, "res$", "r"); // r => res
            }
            else if (s.EndsWith("s"))
            {
                result = System.Text.RegularExpressions.Regex.Replace(s, "s$", ""); // ? => s
            }
            else            
            {
                result = s;
            }
            
            return result;
        }
        
        public string StringSplitPascalCase(string s)
        {
            Regex regex = new Regex("(?<=[a-z])(?<x>[A-Z|0-9|#])|(?<=.)(?<x>[A-Z|0-9|#])(?=[a-z])");

            return regex.Replace(s, " ${x}").Replace("_", " "); // "_" => " " !?!
        }
        
        public string StringToLowerFirstLetter(string s)
        {
            if (s.Length > 1)
            {
                return Char.ToLower(s[0]) + s.Substring(1); // 2.6
                //return Char.ToLowerInvariant(s[0]) + s.Substring(1); // 2.6
            }
            else
            {
                return s.ToLower();
            }
        }
        
        public string StringToUpperFirstLetter(string s)
        {
            if (s.Length > 1)
            {
                return Char.ToUpper(s[0]) + s.Substring(1);
                //return Char.ToUpperInvariant(s[0]) + s.Substring(1); // 2.6
            }
            else
            {
                return s.ToUpper();
            }
        }

        #endregion

        #region Generate

        protected void GenerateTable(string templateFileName, TableSchema table,
            string myNamespace, string myDatabase, string fileName, Cultures culture)
        {
            //System.Diagnostics.Debugger.Break();

            CodeTemplateCompiler compiler = new CodeTemplateCompiler(templateFileName);
            compiler.Compile();

            if (compiler.Errors.Count == 0)
            {
                CodeTemplate template = compiler.CreateInstance();

                template.SetProperty("Culture", culture);
                template.SetProperty("Database", myDatabase);
                template.SetProperty("Namespace", myNamespace);
                template.SetProperty("SourceTable", table);

                template.RenderToFile(fileName, true);
            }
            else
            {
                for (int i = 0; i < compiler.Errors.Count; i++)
                {
                    Console.Error.WriteLine(compiler.Errors[i].ToString());
                }
            }
        }

        protected void GenerateTables(string templateFileName, TableSchemaCollection sourceTables,
            string myNamespace, string myDatabase, string fileName, Cultures culture)
        {
            //System.Diagnostics.Debugger.Break();

            CodeTemplateCompiler compiler = new CodeTemplateCompiler(templateFileName);
            compiler.Compile();

            if (compiler.Errors.Count == 0)
            {
                CodeTemplate template = compiler.CreateInstance();

                template.SetProperty("Culture", culture);
                template.SetProperty("Database", myDatabase);
                template.SetProperty("Namespace", myNamespace);
                template.SetProperty("SourceTables", sourceTables);

                template.RenderToFile(fileName, true);
            }
            else
            {
                for (int i = 0; i < compiler.Errors.Count; i++)
                {
                    Console.Error.WriteLine(compiler.Errors[i].ToString());
                }
            }
        }

        #endregion

        #region Generate Presentation
        public void GeneratePresentationCollectionModel(TableSchemaCollection sourceTables,
            string myNamespace, string myDatabase, string output, Cultures culture,
            string ajax)
        {
            ajax = String.IsNullOrEmpty(ajax) ? "" : ajax;

            string input = this.CodeTemplateInfo.DirectoryName;

            if (output.Trim() == "")
            {
                output = DefaultOutput;
            }

            if (IsNullOrEmpty(myDatabase))
            {
                myDatabase = myNamespace;
            }

            output = output + "/" + myNamespace + ajax + "/Models/" + myDatabase + "/CollectionModels";
            CreateDirectory(output);

            foreach (TableSchema table in sourceTables)
            {
                string className = ClassName(table.FullName, culture);

                GenerateTable(input + "/Presentation/Presentation.Model.CollectionModel.cst", table, myNamespace, myDatabase, output + "/" + className + "CollectionModel.cs", culture);
            }
        }

        public void GeneratePresentationItemModel(TableSchemaCollection sourceTables,
            string myNamespace, string myDatabase, string output, Cultures culture,
            string ajax)
        {
            ajax = String.IsNullOrEmpty(ajax) ? "" : ajax;

            string input = this.CodeTemplateInfo.DirectoryName;

            if (output.Trim() == "")
            {
                output = DefaultOutput;
            }

            if (IsNullOrEmpty(myDatabase))
            {
                myDatabase = myNamespace;
            }

            output = output + "/" + myNamespace + ajax + "/Models/" + myDatabase + "/ItemModels";
            CreateDirectory(output);

            foreach (TableSchema table in sourceTables)
            {
                string className = ClassName(table.FullName, culture);

                GenerateTable(input + "/Presentation/Presentation.Model.ItemModel.cst", table, myNamespace, myDatabase, output + "/" + className + "ItemModel.cs", culture);
            }
        }

        #endregion

        #region Generate Presentation MVC

        public void GeneratePresentationMvcController(TableSchemaCollection sourceTables,
            string myNamespace, string myDatabase, string output, Cultures culture, Archetypes archetype,
            string ajax)
        {
            ajax = String.IsNullOrEmpty(ajax) ? "" : ajax;

            string input = this.CodeTemplateInfo.DirectoryName;
            
            if (output.Trim() == "")
            {
                output = DefaultOutput;
            }
            
            if (IsNullOrEmpty(myDatabase))
            {
                myDatabase = myNamespace;
            }
            
            output = output + "/" + myNamespace + ajax +  "/Controllers" + "/" + myDatabase;
            CreateDirectory(output);

            foreach (TableSchema table in sourceTables)
            {
                string className = ClassName(table.FullName, culture);
                
                if (archetype == Archetypes.Persistence)
                {
                    GenerateTable(input + "/Presentation.MVC" + ajax + "/Presentation.MVC.Controller.Persistence.cst", table, myNamespace, myDatabase, output + "/" + className + "Controller.cs", culture);
                }
                else // Archetypes.Application
                {
                    GenerateTable(input + "/Presentation.MVC" + ajax + "/Presentation.MVC.Controller.Application.cst", table, myNamespace, myDatabase, output + "/" + className + "Controller.cs", culture);
                }
            }
        }
       
        public void GeneratePresentationMvcPartialView(TableSchemaCollection sourceTables,
            string myNamespace, string myDatabase, string output, Cultures culture,
            string ajax)
        {
            ajax = String.IsNullOrEmpty(ajax) ? "" : ajax;

            string input = this.CodeTemplateInfo.DirectoryName;
            
            if (output.Trim() == "")
            {
                output = DefaultOutput;
            }
            
            if (IsNullOrEmpty(myDatabase))
            {
                myDatabase = myNamespace;
            }
            
            output = output + "/" + myNamespace + ajax + "/Views";
            CreateDirectory(output);
            
            output = output + "/" + myDatabase;
            CreateDirectory(output);
                 
            foreach (TableSchema table in sourceTables)
            {
                string className = ClassName(table.FullName, culture);

                string outputC = output + "/" + className;
                CreateDirectory(outputC);
                
                GenerateTable(input + "/Presentation.MVC" + ajax + "/Presentation.MVC.PartialView._Collection.cst", table, myNamespace, myDatabase, outputC + "/_" + className + "Collection.cshtml", culture);  
                GenerateTable(input + "/Presentation.MVC" + ajax + "/Presentation.MVC.PartialView._Item.cst", table, myNamespace, myDatabase, outputC + "/_" + className + "Item.cshtml", culture);
                GenerateTable(input + "/Presentation/Presentation.MVC.PartialView._Lookup.cst", table, myNamespace, myDatabase, outputC + "/_" + className + "Lookup.cshtml", culture);  
            }
        }

        public void GeneratePresentationMvcView(TableSchemaCollection sourceTables,
            string myNamespace, string myDatabase, string output, Cultures culture,
            string ajax)
        {
            ajax = String.IsNullOrEmpty(ajax) ? "" : ajax;
    
            string input = this.CodeTemplateInfo.DirectoryName;
            
            if (output.Trim() == "")
            {
                output = DefaultOutput;
            }
            
            if (IsNullOrEmpty(myDatabase))
            {
                myDatabase = myNamespace;
            }
            
            output = output + "/" + myNamespace + ajax + "/Views";
            CreateDirectory(output);

            output = output + "/" + myDatabase;
            CreateDirectory(output);

            foreach (TableSchema table in sourceTables)
            {
                string className = ClassName(table.FullName, culture);

                string outputC = output + "/" + className;
                CreateDirectory(outputC);
            
                GenerateTable(input + "/Presentation.MVC" + ajax + "/Presentation.MVC.View.Create.cst", table, myNamespace, myDatabase, outputC + "/" + "Create.cshtml", culture);
                GenerateTable(input + "/Presentation.MVC" + ajax + "/Presentation.MVC.View.Delete.cst", table, myNamespace, myDatabase, outputC + "/" + "Delete.cshtml", culture);
                GenerateTable(input + "/Presentation.MVC" + ajax + "/Presentation.MVC.View.Index.cst", table, myNamespace, myDatabase, outputC + "/" + "Index.cshtml", culture);
                GenerateTable(input + "/Presentation.MVC" + ajax + "/Presentation.MVC.View.Read.cst", table, myNamespace, myDatabase, outputC + "/" + "Read.cshtml", culture);
                GenerateTable(input + "/Presentation.MVC" + ajax + "/Presentation.MVC.View.Update.cst", table, myNamespace, myDatabase, outputC + "/" + "Update.cshtml", culture);
            }
        }

        #endregion

        #region Generate Service

        public void GenerateServiceODataController(TableSchemaCollection sourceTables,
            string myNamespace, string myDatabase, string output, Cultures culture, Archetypes archetype)
        {
            string input = this.CodeTemplateInfo.DirectoryName;
            
            if (output.Trim() == "")
            {
                output = DefaultOutput;
            }
            
            if (IsNullOrEmpty(myDatabase))
            {
                myDatabase = myNamespace;
            }
            
            output = output + "/" + myNamespace + "/Controllers" + "/" + myDatabase;
            CreateDirectory(output);

            foreach (TableSchema table in sourceTables)
            {
                string className = ClassName(table.FullName, culture);
                
                if (archetype == Archetypes.Persistence)
                {
                    GenerateTable(input + "/Service.OData/Service.OData.Controller.Persistence.cst", table, myNamespace, myDatabase, output + "/" + className + "Controller.cs", culture);
                }
                else // Archetypes.Application
                {
                    GenerateTable(input + "/Service.OData/Service.OData.Controller.Application.cst", table, myNamespace, myDatabase, output + "/" + className + "Controller.cs", culture);
                }
            }
        }
        
        public void GenerateServiceWebApiController(TableSchemaCollection sourceTables,
            string myNamespace, string myDatabase, string output, Cultures culture, Archetypes archetype)
        {
            string input = this.CodeTemplateInfo.DirectoryName;
            
            if (output.Trim() == "")
            {
                output = DefaultOutput;
            }
            
            if (IsNullOrEmpty(myDatabase))
            {
                myDatabase = myNamespace;
            }
            
            output = output + "/" + myNamespace + "/Controllers" + "/" + myDatabase;
            CreateDirectory(output);

            foreach (TableSchema table in sourceTables)
            {
                string className = ClassName(table.FullName, culture);
                
                if (archetype == Archetypes.Persistence)
                {
                    GenerateTable(input + "/Service.WebAPI/Service.WebAPI.Controller.Persistence.cst", table, myNamespace, myDatabase, output + "/" + className + "Controller.cs", culture);
                }
                else // Archetypes.Application
                {
                    GenerateTable(input + "/Service.WebAPI/Service.WebAPI.Controller.Application.cst", table, myNamespace, myDatabase, output + "/" + className + "Controller.cs", culture);
                }
            }
        }
        
        #endregion
        
        #region Generate Application
        
        public void GenerateApplication(TableSchemaCollection sourceTables,
            string myNamespace, string myDatabase, string output, Cultures culture)
        {
            string input = this.CodeTemplateInfo.DirectoryName;
            
            if (output.Trim() == "")
            {
                output = DefaultOutput;
            }
            
            if (IsNullOrEmpty(myDatabase))
            {
                myDatabase = myNamespace;
            }

            output = output + "/" + myNamespace;
            CreateDirectory(output);
            string outputI = output + "/Interfaces";
            CreateDirectory(outputI);
            
            GenerateTables(input + "/Application/Application.IGenericApplication.cst", sourceTables, myNamespace, myDatabase, outputI + "/I" + myDatabase + "GenericApplication.cs", culture);
            GenerateTables(input + "/Application/Application.IGenericApplicationDTO.cst", sourceTables, myNamespace, myDatabase, outputI + "/I" + myDatabase + "GenericApplicationDTO.cs", culture);
            GenerateTables(input + "/Application/Application.GenericApplication.cst", sourceTables, myNamespace, myDatabase, output + "/" + myDatabase + "GenericApplication.cs", culture);
            GenerateTables(input + "/Application/Application.GenericApplicationDTO.cst", sourceTables, myNamespace, myDatabase, output + "/" + myDatabase + "GenericApplicationDTO.cs", culture);
        }

        #endregion

        #region Generate Data       

        public void GenerateDataDataModel(TableSchemaCollection sourceTables,
            string myNamespace, string myDatabase, string output, Cultures culture)
        {
            string input = this.CodeTemplateInfo.DirectoryName;
            
            if (output.Trim() == "")
            {
                output = DefaultOutput;
            }
            
            if (IsNullOrEmpty(myDatabase))
            {
                myDatabase = myNamespace;
            }

            output = output + "/" + myNamespace + "/DataModels";
            CreateDirectory(output);            

            foreach (TableSchema table in sourceTables)
            {
                string className = ClassName(table.FullName, culture);
                
                GenerateTable(input + "/Data/Data.DataModel.cst", table, myNamespace, myDatabase, output + "/" + className + ".cs", culture);
            }
        }

        public void GenerateDataDTO(TableSchemaCollection sourceTables,
            string myNamespace, string myDatabase, string output, Cultures culture)
        {
            string input = this.CodeTemplateInfo.DirectoryName;
            
            if (output.Trim() == "")
            {
                output = DefaultOutput;
            }
            
            if (IsNullOrEmpty(myDatabase))
            {
                myDatabase = myNamespace;
            }

            output = output + "/" + myNamespace + "/DTOs";
            CreateDirectory(output);

            foreach (TableSchema table in sourceTables)
            {
                string className = ClassName(table.FullName, culture);
                
                GenerateTable(input + "/Data/Data.DTO.cst", table, myNamespace, myDatabase, output + "/" + className + "DTO.cs", culture);
            }
        } 
                
        public void GenerateDataResource(TableSchemaCollection sourceTables,
            string myNamespace, string myDatabase, string output, Cultures culture)
        {
            string input = this.CodeTemplateInfo.DirectoryName;
            
            if (output.Trim() == "")
            {
                output = DefaultOutput;
            }
            
            if (IsNullOrEmpty(myDatabase))
            {
                myDatabase = myNamespace;
            }
            
            output = output + "/" + myNamespace + "/Resources";
            CreateDirectory(output);            

            foreach (TableSchema table in sourceTables)
            {
                string className = ClassName(table.FullName, culture);
                
                GenerateTable(input + "/Data/Data.Resource.cst", table, myNamespace, myDatabase, output + "/" + className + "Resources.resx", culture);
            }
        }

        public void GenerateDataViewModel(TableSchemaCollection sourceTables,
            string myNamespace, string myDatabase, string output, Cultures culture)
        {
            string input = this.CodeTemplateInfo.DirectoryName;

            if (output.Trim() == "")
            {
                output = DefaultOutput;
            }

            if (IsNullOrEmpty(myDatabase))
            {
                myDatabase = myNamespace;
            }

            output = output + "/" + myNamespace + "/ViewModels";
            CreateDirectory(output);

            foreach (TableSchema table in sourceTables)
            {
                string className = ClassName(table.FullName, culture);

                GenerateTable(input + "/Data/Data.ViewModel.cst", table, myNamespace, myDatabase, output + "/" + className + "ViewModel.cs", culture);
            }
        }

        #endregion
        
        #region Generate Persistence
        
        public void GeneratePersistence(TableSchemaCollection sourceTables,
            string myNamespace, string myDatabase, string output, Cultures culture)
        {
            string input = this.CodeTemplateInfo.DirectoryName;
            
            if (output.Trim() == "")
            {
                output = DefaultOutput;
            }
            
            if (IsNullOrEmpty(myDatabase))
            {
                myDatabase = myNamespace;
            }

            output = output + "/" + myNamespace;
            CreateDirectory(output);
            string outputI = output + "/Interfaces";
            CreateDirectory(outputI);
            
            GenerateTables(input + "/Persistence/Persistence.IGenericRepository.cst", sourceTables, myNamespace, myDatabase, outputI + "/I" + myDatabase + "GenericRepository.cs", culture);
            GenerateTables(input + "/Persistence/Persistence.IGenericRepositoryDTO.cst", sourceTables, myNamespace, myDatabase, outputI + "/I" + myDatabase + "GenericRepositoryDTO.cs", culture);
            GenerateTables(input + "/Persistence/Persistence.IUnitOfWork.cst", sourceTables, myNamespace, myDatabase, outputI + "/I" + myDatabase + "UnitOfWork.cs", culture);
            GenerateTables(input + "/Persistence/Persistence.IUnitOfWorkDTO.cst", sourceTables, myNamespace, myDatabase, outputI + "/I" + myDatabase + "UnitOfWorkDTO.cs", culture);
        }

        #endregion

        #region Generate Persistence Entity Framework

        public void GeneratePersistenceEntityFrameworkConfiguration(TableSchemaCollection sourceTables,
            string myNamespace, string myDatabase, string output, Cultures culture)
        {
            string input = this.CodeTemplateInfo.DirectoryName;
            
            if (output.Trim() == "")
            {
                output = DefaultOutput;
            }
            
            if (IsNullOrEmpty(myDatabase))
            {
                myDatabase = myNamespace;
            }

            output = output + "/" + myNamespace + "EntityFramework" + "/Configurations";
            CreateDirectory(output);

            foreach (TableSchema table in sourceTables)
            {
                string className = ClassName(table.FullName, culture);
                
                GenerateTable(input + "/PersistenceEntityFramework/PersistenceEntityFramework.Configuration.cst", table, myNamespace, myDatabase, output + "/" + className + "Configuration.cs", culture);
            }
        }

        public void GeneratePersistenceEntityFrameworkDbContext(TableSchemaCollection sourceTables,
            string myNamespace, string myDatabase, string output, Cultures culture)
        {
            string input = this.CodeTemplateInfo.DirectoryName;
            
            if (output.Trim() == "")
            {
                output = DefaultOutput;
            }
            
            if (IsNullOrEmpty(myDatabase))
            {
                myDatabase = myNamespace;
            }

            output = output + "/" + myNamespace + "EntityFramework";
            CreateDirectory(output);
            
            GenerateTables(input + "/PersistenceEntityFramework/PersistenceEntityFramework.DbContext.cst", sourceTables, myNamespace, myDatabase, output + "/" + myDatabase + "DbContext.cs", culture);
        }

        public void GeneratePersistenceEntityFrameworkGenericRepository(TableSchemaCollection sourceTables,
            string myNamespace, string myDatabase, string output, Cultures culture)
        {
            string input = this.CodeTemplateInfo.DirectoryName;
            
            if (output.Trim() == "")
            {
                output = DefaultOutput;
            }
            
            if (IsNullOrEmpty(myDatabase))
            {
                myDatabase = myNamespace;
            }

            output = output + "/" + myNamespace + "EntityFramework" + "/UnitOfWork";
            CreateDirectory(output);

            GenerateTables(input + "/PersistenceEntityFramework/PersistenceEntityFramework.GenericRepository.cst", sourceTables, myNamespace, myDatabase, output + "/" + myDatabase + "GenericRepositoryEF.cs", culture);
        }

        public void GeneratePersistenceEntityFrameworkUnitOfWork(TableSchemaCollection sourceTables,
            string myNamespace, string myDatabase, string output, Cultures culture)
        {
            string input = this.CodeTemplateInfo.DirectoryName;
            
            if (output.Trim() == "")
            {
                output = DefaultOutput;
            }
            
            if (IsNullOrEmpty(myDatabase))
            {
                myDatabase = myNamespace;
            }

            output = output + "/" + myNamespace + "EntityFramework" + "/UnitOfWork";
            CreateDirectory(output);

            GenerateTables(input + "/PersistenceEntityFramework/PersistenceEntityFramework.UnitOfWork.cst", sourceTables, myNamespace, myDatabase, output + "/" + myDatabase + "UnitOfWorkEF.cs", culture);
        }
        
        #endregion

        #region Generate Persistence LINQ2DB

        public void GeneratePersistenceLINQ2DBDataConnection(TableSchemaCollection sourceTables,
            string myNamespace, string myDatabase, string output, Cultures culture)
        {
            string input = this.CodeTemplateInfo.DirectoryName;
            
            if (output.Trim() == "")
            {
                output = DefaultOutput;
            }
            
            if (IsNullOrEmpty(myDatabase))
            {
                myDatabase = myNamespace;
            }

            output = output + "/" + myNamespace + "LINQ2DB";
            CreateDirectory(output);
            
            GenerateTables(input + "/PersistenceLINQ2DB/PersistenceLINQ2DB.DataConnection.cst", sourceTables, myNamespace, myDatabase, output + "/" + myDatabase + "LINQ2DB.cs", culture);
        }        

        public void GeneratePersistenceLINQ2DBGenericRepository(TableSchemaCollection sourceTables,
            string myNamespace, string myDatabase, string output, Cultures culture)
        {
            string input = this.CodeTemplateInfo.DirectoryName;
            
            if (output.Trim() == "")
            {
                output = DefaultOutput;
            }
            
            if (IsNullOrEmpty(myDatabase))
            {
                myDatabase = myNamespace;
            }

            output = output + "/" + myNamespace + "LINQ2DB" + "/UnitOfWork";
            CreateDirectory(output);

            GenerateTables(input + "/PersistenceLINQ2DB/PersistenceLINQ2DB.GenericRepository.cst", sourceTables, myNamespace, myDatabase, output + "/" + myDatabase + "GenericRepositoryLINQ2DB.cs", culture);
        }

        public void GeneratePersistenceLINQ2DBMap(TableSchemaCollection sourceTables,
            string myNamespace, string myDatabase, string output, Cultures culture)
        {
            string input = this.CodeTemplateInfo.DirectoryName;
            
            if (output.Trim() == "")
            {
                output = DefaultOutput;
            }
            
            if (IsNullOrEmpty(myDatabase))
            {
                myDatabase = myNamespace;
            }

            output = output + "/" + myNamespace + "LINQ2DB" + "/Maps";
            CreateDirectory(output);

            foreach (TableSchema table in sourceTables)
            {
                string className = ClassName(table.FullName, culture);
                
                GenerateTable(input + "/PersistenceLINQ2DB/PersistenceLINQ2DB.Map.cst", table, myNamespace, myDatabase, output + "/" + className + "Map.cs", culture);
            }
        }

        public void GeneratePersistenceLINQ2DBRepository(TableSchemaCollection sourceTables,
            string myNamespace, string myDatabase, string output, Cultures culture)
        {
            string input = this.CodeTemplateInfo.DirectoryName;
            
            if (output.Trim() == "")
            {
                output = DefaultOutput;
            }
            
            if (IsNullOrEmpty(myDatabase))
            {
                myDatabase = myNamespace;
            }

            output = output + "/" + myNamespace + "LINQ2DB" + "/Repositories";
            CreateDirectory(output);

            foreach (TableSchema table in sourceTables)
            {
                string className = ClassName(table.FullName, culture);
                
                GenerateTable(input + "/PersistenceLINQ2DB/PersistenceLINQ2DB.Repository.cst", table, myNamespace, myDatabase, output + "/" + myDatabase + className + "RepositoryLINQ2DB.cs", culture);
            }
        }
        
        public void GeneratePersistenceLINQ2DBUnitOfWork(TableSchemaCollection sourceTables,
            string myNamespace, string myDatabase, string output, Cultures culture)
        {
            string input = this.CodeTemplateInfo.DirectoryName;
            
            if (output.Trim() == "")
            {
                output = DefaultOutput;
            }
            
            if (IsNullOrEmpty(myDatabase))
            {
                myDatabase = myNamespace;
            }

            output = output + "/" + myNamespace + "LINQ2DB" + "/UnitOfWork";
            CreateDirectory(output);

            GenerateTables(input + "/PersistenceLINQ2DB/PersistenceLINQ2DB.UnitOfWork.cst", sourceTables, myNamespace, myDatabase, output + "/" + myDatabase + "UnitOfWorkLINQ2DB.cs", culture);
        }

        #endregion

        #region Generate Persistence MongoDB

        public void GeneratePersistenceMongoDBMap(TableSchemaCollection sourceTables,
            string myNamespace, string myDatabase, string output, Cultures culture)
        {
            string input = this.CodeTemplateInfo.DirectoryName;
            
            if (output.Trim() == "")
            {
                output = DefaultOutput;
            }
            
            if (IsNullOrEmpty(myDatabase))
            {
                myDatabase = myNamespace;
            }

            output = output + "/" + myNamespace + "MongoDB" + "/Maps";
            CreateDirectory(output);

            foreach (TableSchema table in sourceTables)
            {
                string className = ClassName(table.FullName, culture);
                
                GenerateTable(input + "/PersistenceMongoDB/PersistenceMongoDB.Map.cst", table, myNamespace, myDatabase, output + "/" + className + "Map.cs", culture);
            }
        }        

        public void GeneratePersistenceMongoDBGenericRepository(TableSchemaCollection sourceTables,
            string myNamespace, string myDatabase, string output, Cultures culture)
        {
            string input = this.CodeTemplateInfo.DirectoryName;
            
            if (output.Trim() == "")
            {
                output = DefaultOutput;
            }
            
            if (IsNullOrEmpty(myDatabase))
            {
                myDatabase = myNamespace;
            }

            output = output + "/" + myNamespace + "MongoDB" + "/UnitOfWork";
            CreateDirectory(output);

            GenerateTables(input + "/PersistenceMongoDB/PersistenceMongoDB.GenericRepository.cst", sourceTables, myNamespace, myDatabase, output + "/" + myDatabase + "GenericRepositoryMongoDB.cs", culture);
        }

        public void GeneratePersistenceMongoDBRepository(TableSchemaCollection sourceTables,
            string myNamespace, string myDatabase, string output, Cultures culture)
        {
            string input = this.CodeTemplateInfo.DirectoryName;

            if (output.Trim() == "")
            {
                output = DefaultOutput;
            }

            if (IsNullOrEmpty(myDatabase))
            {
                myDatabase = myNamespace;
            }

            output = output + "/" + myNamespace + "MongoDB" + "/Repositories";
            CreateDirectory(output);

            foreach (TableSchema table in sourceTables)
            {
                string className = ClassName(table.FullName, culture);

                GenerateTable(input + "/PersistenceMongoDB/PersistenceMongoDB.Repository.cst", table, myNamespace, myDatabase, output + "/" + myDatabase + className + "RepositoryMongoDB.cs", culture);
            }
        }

        public void GeneratePersistenceMongoDBUnitOfWork(TableSchemaCollection sourceTables,
            string myNamespace, string myDatabase, string output, Cultures culture)
        {
            string input = this.CodeTemplateInfo.DirectoryName;
            
            if (output.Trim() == "")
            {
                output = DefaultOutput;
            }
            
            if (IsNullOrEmpty(myDatabase))
            {
                myDatabase = myNamespace;
            }

            output = output + "/" + myNamespace + "MongoDB" + "/UnitOfWork";
            CreateDirectory(output);

            GenerateTables(input + "/PersistenceMongoDB/PersistenceMongoDB.UnitOfWork.cst", sourceTables, myNamespace, myDatabase, output + "/" + myDatabase + "UnitOfWorkMongoDB.cs", culture);
        }

        #endregion

        #region Generate Persistence NHibernate

        public void GeneratePersistenceNHibernateMap(TableSchemaCollection sourceTables,
            string myNamespace, string myDatabase, string output, Cultures culture)
        {
            string input = this.CodeTemplateInfo.DirectoryName;
            
            if (output.Trim() == "")
            {
                output = DefaultOutput;
            }
            
            if (IsNullOrEmpty(myDatabase))
            {
                myDatabase = myNamespace;
            }

            output = output + "/" + myNamespace + "NHibernate" + "/Maps";
            CreateDirectory(output);

            foreach (TableSchema table in sourceTables)
            {
                string className = ClassName(table.FullName, culture);
                
                GenerateTable(input + "/PersistenceNHibernate/PersistenceNHibernate.Map.cst", table, myNamespace, myDatabase, output + "/" + className + "Map.cs", culture);
            }
        }        

        public void GeneratePersistenceNHibernateFactory(TableSchemaCollection sourceTables,
            string myNamespace, string myDatabase, string output, Cultures culture)
        {
            string input = this.CodeTemplateInfo.DirectoryName;
            
            if (output.Trim() == "")
            {
                output = DefaultOutput;
            }
            
            if (IsNullOrEmpty(myDatabase))
            {
                myDatabase = myNamespace;
            }

            output = output + "/" + myNamespace + "NHibernate";
            CreateDirectory(output);
            
            GenerateTables(input + "/PersistenceNHibernate/PersistenceNHibernate.Factory.cst", sourceTables, myNamespace, myDatabase, output + "/" + myDatabase + "Factory.cs", culture);
        }

        public void GeneratePersistenceNHibernateGenericRepository(TableSchemaCollection sourceTables,
            string myNamespace, string myDatabase, string output, Cultures culture)
        {
            string input = this.CodeTemplateInfo.DirectoryName;
            
            if (output.Trim() == "")
            {
                output = DefaultOutput;
            }
            
            if (IsNullOrEmpty(myDatabase))
            {
                myDatabase = myNamespace;
            }

            output = output + "/" + myNamespace + "NHibernate" + "/UnitOfWork";
            CreateDirectory(output);

            GenerateTables(input + "/PersistenceNHibernate/PersistenceNHibernate.GenericRepository.cst", sourceTables, myNamespace, myDatabase, output + "/" + myDatabase + "GenericRepositoryNH.cs", culture);
        }

        public void GeneratePersistenceNHibernateUnitOfWork(TableSchemaCollection sourceTables,
            string myNamespace, string myDatabase, string output, Cultures culture)
        {
            string input = this.CodeTemplateInfo.DirectoryName;
            
            if (output.Trim() == "")
            {
                output = DefaultOutput;
            }
            
            if (IsNullOrEmpty(myDatabase))
            {
                myDatabase = myNamespace;
            }

            output = output + "/" + myNamespace + "NHibernate" + "/UnitOfWork";
            CreateDirectory(output);

            GenerateTables(input + "/PersistenceNHibernate/PersistenceNHibernate.UnitOfWork.cst", sourceTables, myNamespace, myDatabase, output + "/" + myDatabase + "UnitOfWorkNH.cs", culture);
        }
        
        #endregion
                
        #region Generate Persistence OData

        public void GeneratePersistenceODataDTO(TableSchemaCollection sourceTables,
            string myNamespace, string myDatabase, string output, Cultures culture)
        {
            string input = this.CodeTemplateInfo.DirectoryName;
            
            if (output.Trim() == "")
            {
                output = DefaultOutput;
            }
            
            if (IsNullOrEmpty(myDatabase))
            {
                myDatabase = myNamespace;
            }

            output = output + "/" + myNamespace + "OData" + "/DTOs";
            CreateDirectory(output);

            foreach (TableSchema table in sourceTables)
            {
                string className = ClassName(table.FullName, culture);
                
                GenerateTable(input + "/PersistenceOData/PersistenceOData.DTO.cst", table, myNamespace, myDatabase, output + "/" + className + "DTO.cs", culture);
            }
        }

        public void GeneratePersistenceODataGenericRepository(TableSchemaCollection sourceTables,
            string myNamespace, string myDatabase, string output, Cultures culture)
        {
            string input = this.CodeTemplateInfo.DirectoryName;
            
            if (output.Trim() == "")
            {
                output = DefaultOutput;
            }
            
            if (IsNullOrEmpty(myDatabase))
            {
                myDatabase = myNamespace;
            }

            output = output + "/" + myNamespace + "OData" + "/UnitOfWork";
            CreateDirectory(output);

            GenerateTables(input + "/PersistenceOData/PersistenceOData.GenericRepository.cst", sourceTables, myNamespace, myDatabase, output + "/" + myDatabase + "GenericRepositoryOData.cs", culture);
        }

        public void GeneratePersistenceODataUnitOfWork(TableSchemaCollection sourceTables,
            string myNamespace, string myDatabase, string output, Cultures culture)
        {
            string input = this.CodeTemplateInfo.DirectoryName;
            
            if (output.Trim() == "")
            {
                output = DefaultOutput;
            }
            
            if (IsNullOrEmpty(myDatabase))
            {
                myDatabase = myNamespace;
            }

            output = output + "/" + myNamespace + "OData" + "/UnitOfWork";
            CreateDirectory(output);

            GenerateTables(input + "/PersistenceOData/PersistenceOData.UnitOfWork.cst", sourceTables, myNamespace, myDatabase, output + "/" + myDatabase + "UnitOfWorkOData.cs", culture);
        }

        #endregion

        #region Generate Persistence RavenDB

        public void GeneratePersistenceRavenDBGenericRepository(TableSchemaCollection sourceTables,
            string myNamespace, string myDatabase, string output, Cultures culture)
        {
            string input = this.CodeTemplateInfo.DirectoryName;

            if (output.Trim() == "")
            {
                output = DefaultOutput;
            }

            if (IsNullOrEmpty(myDatabase))
            {
                myDatabase = myNamespace;
            }

            output = output + "/" + myNamespace + "RavenDB" + "/UnitOfWork";
            CreateDirectory(output);

            GenerateTables(input + "/PersistenceRavenDB/PersistenceRavenDB.GenericRepository.cst", sourceTables, myNamespace, myDatabase, output + "/" + myDatabase + "GenericRepositoryRavenDB.cs", culture);
        }

        public void GeneratePersistenceRavenDBUnitOfWork(TableSchemaCollection sourceTables,
            string myNamespace, string myDatabase, string output, Cultures culture)
        {
            string input = this.CodeTemplateInfo.DirectoryName;

            if (output.Trim() == "")
            {
                output = DefaultOutput;
            }

            if (IsNullOrEmpty(myDatabase))
            {
                myDatabase = myNamespace;
            }

            output = output + "/" + myNamespace + "RavenDB" + "/UnitOfWork";
            CreateDirectory(output);

            GenerateTables(input + "/PersistenceRavenDB/PersistenceRavenDB.UnitOfWork.cst", sourceTables, myNamespace, myDatabase, output + "/" + myDatabase + "UnitOfWorkRavenDB.cs", culture);
        }

        #endregion
        
        #region Generate Persistence Redis

        public void GeneratePersistenceRedisGenericRepository(TableSchemaCollection sourceTables,
            string myNamespace, string myDatabase, string output, Cultures culture)
        {
            string input = this.CodeTemplateInfo.DirectoryName;

            if (output.Trim() == "")
            {
                output = DefaultOutput;
            }

            if (IsNullOrEmpty(myDatabase))
            {
                myDatabase = myNamespace;
            }

            output = output + "/" + myNamespace + "Redis" + "/UnitOfWork";
            CreateDirectory(output);

            GenerateTables(input + "/PersistenceRedis/PersistenceRedis.GenericRepository.cst", sourceTables, myNamespace, myDatabase, output + "/" + myDatabase + "GenericRepositoryRedis.cs", culture);
        }

        public void GeneratePersistenceRedisRepository(TableSchemaCollection sourceTables,
            string myNamespace, string myDatabase, string output, Cultures culture)
        {
            string input = this.CodeTemplateInfo.DirectoryName;

            if (output.Trim() == "")
            {
                output = DefaultOutput;
            }

            if (IsNullOrEmpty(myDatabase))
            {
                myDatabase = myNamespace;
            }

            output = output + "/" + myNamespace + "Redis" + "/Repositories";
            CreateDirectory(output);

            foreach (TableSchema table in sourceTables)
            {
                string className = ClassName(table.FullName, culture);

                GenerateTable(input + "/PersistenceRedis/PersistenceRedis.Repository.cst", table, myNamespace, myDatabase, output + "/" + myDatabase + className + "RepositoryRedis.cs", culture);
            }
        }

        public void GeneratePersistenceRedisUnitOfWork(TableSchemaCollection sourceTables,
            string myNamespace, string myDatabase, string output, Cultures culture)
        {
            string input = this.CodeTemplateInfo.DirectoryName;

            if (output.Trim() == "")
            {
                output = DefaultOutput;
            }

            if (IsNullOrEmpty(myDatabase))
            {
                myDatabase = myNamespace;
            }

            output = output + "/" + myNamespace + "Redis" + "/UnitOfWork";
            CreateDirectory(output);

            GenerateTables(input + "/PersistenceRedis/PersistenceRedis.UnitOfWork.cst", sourceTables, myNamespace, myDatabase, output + "/" + myDatabase + "UnitOfWorkRedis.cs", culture);
        }

        #endregion
        
        #region Tables & Columns
        
        // SQL
        
        public string TableName(string name)
        {
            if (IsCase)
            {
                return name.Replace("dbo.", "");                
            }
            else
            {            
                return name.Replace("dbo.", "");
            }
        }
        
        public string TableAlias(string name)
        {
            if (IsCase)
            {
                return TableName(name).Replace(".", "_");
            }
            else
            {            
                return TableName(name).Replace(".", "_");
            }
        }
        
        public string ColumnName(string name)
        {
            if (IsCase)
            {
                return name;                
            }
            else
            {            
                return name;
            }
        }
        
        // Class
        
        public string ClassLabel(string name)
        {
            return ClassLabel(name, false);
        }
        
        public string ClassLabel(string name, bool isLower)
        {
            bool isUnderscore = false;
            return ClassWords(name, isLower, ref isUnderscore);
        }
        
        public string ClassName(string name, Cultures culture)
        {
            bool isLower = false;
            bool isUnderscore = false;
            string result = ClassWords(name, isLower, ref isUnderscore);            
            if (isUnderscore)
            {
                return Singular(result.Replace(" ", "_"), culture); // Singular
            }
            else
            {
                return Singular(result.Replace(" ", ""), culture); // Singular
            }
        }
        
        public string ObjectName(string name, Cultures culture)
        {
            bool isLower = true;
            bool isUnderscore = false;
            string result = ClassWords(name, isLower, ref isUnderscore);

            if (isUnderscore)
            {
                return Singular(result.Replace(" ", "_"), culture); // Singular
            }
            else
            {
                return Singular(result.Replace(" ", ""), culture); // Singular
            }
        }

        public string ClassWords(string name, bool isLower, ref bool isUnderscore)
        {
            string result;
            string[] words;
            
            name = name.Replace("aspnet_", "").Replace("AspNet", "").Replace(" ", ""); // SPACEs are not allowed !
            
            isUnderscore = false;

            // Schema.Table => Table
            words = name.Split('.');
            if (words.Length > 1)
            {
                name = words[1];
            }            
            
            if (IsCase)
            {
                //result = "{" + name + "} " + " [" + StringSplitPascalCase(name) + "]" + " [" + StringSplitPascalCase("UsersInRoles") + "]";

                if (name.IndexOf('_') >= 0)
                {
                    isUnderscore = true;
                    words = name.Split('_');
                }
                else
                {
                    words = StringSplitPascalCase(name).Split(' ');
                }
                
                result = "";
                int index = 0;
                foreach (string word in words)
                {   
                    if (isLower && index == 0)
                    {
                        result += (!IsNullOrEmpty(result) ? " " : "") + word.ToLower();
                    }
                    else if (Array.IndexOf(Acronyms, word) >= 0) // Is an Acronym
                    {
                        result += (!IsNullOrEmpty(result) ? " " : "") + word;
                    }
                    else
                    {
                        result += (!IsNullOrEmpty(result) ? " " : "") + StringToUpperFirstLetter(word.ToLower());
                    }
                    index++;
                }
            }
            else
            {
                if (name.IndexOf('_') >= 0)
                {
                    isUnderscore = true;
                    words = name.Split('_');
                }                    
                else
                {
                    words = StringSplitPascalCase(name.Replace("dbo.", "")).Split(' ');
                }                

                if(isLower)
                {
                    words[0] = StringToLowerFirstLetter(words[0]);
                }

                result = String.Join(" ", words);
            }
                    
            return result;
        }
        
        // Property
        
        public string PropertyLabel(string name)
        {
            return PropertyLabel(name, false);
        }
        
        public string PropertyLabel(string name, bool isLower)
        {
            bool isUnderscore = false;
            return PropertyWords(name, isLower, ref isUnderscore);
        }

        public string PropertyName(string name)
        {
            bool isLower = false;
            bool isUnderscore = false;
            string result = PropertyWords(name, isLower, ref isUnderscore);            
            if (isUnderscore)
            {
                return result.Replace(" ", "_");
            }
            else
            {
                return result.Replace(" ", "");
            }
        }
        
        public string LocalName(string name)
        {
            bool isLower = true;
            bool isUnderscore = false;
            string result = PropertyWords(name, isLower, ref isUnderscore);            
            if (isUnderscore)
            {
                return result.Replace(" ", "_");
            }
            else
            {
                return result.Replace(" ", "");
            }
        }
        
        public string PropertyWords(string name, bool isLower, ref bool isUnderscore)
        {
            string result;
            string[] words;            
            
            name = name.Replace("aspnet_", "").Replace("AspNet", "");

            isUnderscore = false;
            
            if (IsCase)            
            {
                if (name.IndexOf('_') >= 0)
                {
                    isUnderscore = true;
                    words = name.Split('_');
                }
                else
                {
                    words = StringSplitPascalCase(name).Split(' ');
                }                

                result = "";
                int index = 0;
                foreach (string word in words)
                {
                    if (isLower && index == 0)
                    {
                        result += (!IsNullOrEmpty(result) ? " " : "") + word.ToLower();
                    }
                    else if (Array.IndexOf(Acronyms, word) >= 0) // Is an Acronym
                    {
                        result += (!IsNullOrEmpty(result) ? " " : "") + word;
                    }
                    else
                    {
                        result += (!IsNullOrEmpty(result) ? " " : "") + StringToUpperFirstLetter(word.ToLower());                    
                    }
                    
                    index++;
                }
            }
            else
            {
                if (name.IndexOf('_') >= 0)
                {
                    isUnderscore = true;
                    words = name.Split('_');
                }                    
                else
                {
                    words = StringSplitPascalCase(name.Replace("dbo.", "")).Split(' ');
                }                

                if(isLower)
                {
                    words[0] = StringToLowerFirstLetter(words[0]);
                }

                result = String.Join(" ", words);                
            }
            
            return result;
        }
        
        #endregion

        #region Type
        
        public bool IsBinary(DbType dbType)
        {
            return (dbType == DbType.Binary
                || dbType == DbType.Object);
        }

        public bool IsBoolean(DbType dbType)
        {
            return (dbType == DbType.Boolean);
        }

        public bool IsDate(DbType dbType)
        {
            return (dbType == DbType.Date);
        }

        public bool IsDateTime(DbType dbType)
        {
            return (dbType == DbType.DateTime);
            //    || dbType == DbType.DateTime2 // 2.6
            //    || dbType == DbType.DateTimeOffset) // 2.6
        }

        public bool IsTime(DbType dbType)
        {
            return (dbType == DbType.Time); 
        }

        public bool IsDecimal(DbType dbType)
        {
            return (dbType == DbType.Currency
                || dbType == DbType.Decimal);
        }

        public bool IsFloat(DbType dbType)
        {
            return (dbType == DbType.Double
                || dbType == DbType.Single);
        }
        /*
        public bool IsDouble(DbType dbType)
        {
            return (dbType == DbType.Double);
        }

        public bool IsSingle(DbType dbType)
        {
            return (dbType == DbType.Single);
        }
         */
        public bool IsGuid(DbType dbType)
        {
            return (dbType == DbType.Guid);
        }
        
        public bool IsInteger(DbType dbType)
        {
            return (dbType == DbType.Byte
                || dbType == DbType.SByte
                || dbType == DbType.Int16
                || dbType == DbType.Int32
                || dbType == DbType.Int64
                || dbType == DbType.UInt16
                || dbType == DbType.UInt32
                || dbType == DbType.UInt64);
        }
        /*
        public bool IsInteger8(DbType dbType) // sbyte
        {
            return (dbType == DbType.Byte
                || dbType == DbType.SByte);
        }

        public bool IsInteger16(DbType dbType) // short
        {
            return (dbType == DbType.Int16
                || dbType == DbType.UInt16);
        }

        public bool IsInteger32(DbType dbType) // int
        {
            return (dbType == DbType.Int32
                || dbType == DbType.UInt32);
        }

        public bool IsInteger64(DbType dbType) // long
        {
            return (dbType == DbType.Int64
                || dbType == DbType.UInt64);
        }
        
        public bool IsObject(DbType dbType)
        {
            return (dbType == DbType.Object);
        }
         */
        public bool IsString(DbType dbType)
        {
            return (dbType == DbType.AnsiString
                || dbType == DbType.AnsiStringFixedLength
                || dbType == DbType.String
                || dbType == DbType.StringFixedLength);
                //|| dbType == DbType.Xml); // 2.6
        }

        public string GetDefault(DbType dbType)
        {
            string result = "";

            if (IsDate(dbType))
                result = "Default_Date";
            else if (IsDateTime(dbType))
                result = "Default_DateTime";
            else if (IsDecimal(dbType))
                result = "Default_Float";
            else if (IsFloat(dbType))
                result = "Default_Float";
            else if (IsInteger(dbType))
                result = "Default_Integer";
            else if (IsString(dbType))
                result = "Default_String";
            else
                result = "Default_String";

            return result;
        }
        
        public string GetFormat(DbType dbType)
        {
            string result = "";
                        
            if (IsDate(dbType))
                result = "Format_Date";
            else if (IsDateTime(dbType))
                result = "Format_DateTime";
            else if (IsDecimal(dbType))
                result = "Format_Float";
            else if (IsFloat(dbType))
                result = "Format_Float";
            else if (IsInteger(dbType))
                result = "Format_Integer";
            else if (IsString(dbType))
                result = "Format_String";
            else
                result = "Format_String";
            
            return result;                
        }

        public string GetDbType(DbType dbType)
        {
            switch (dbType)
            {
                case DbType.AnsiString: return "String";
                case DbType.AnsiStringFixedLength: return "String";
                case DbType.Binary: return "Binary";
                case DbType.Boolean: return "Boolean";
                case DbType.Byte: return "Byte";
                //case DbType.Currency: return "Currency";
                case DbType.Currency: return "Decimal";
                case DbType.Date: return "DateTime";
                case DbType.DateTime: return "DateTime";
                //case DbType.DateTime2: return ""; // 2.6
                //case DbType.DateTimeOffset: return ""; // 2.6
                case DbType.Decimal: return "Decimal";
                case DbType.Double: return "Double";
                case DbType.Guid: return "Guid";
                case DbType.Int16: return "Int16";
                case DbType.Int32: return "Int32";
                case DbType.Int64: return "Int64";
                //case DbType.Object: return "Object";
                case DbType.Object: return "Binary";
                //case DbType.SByte: return "SByte"; // siegmar
                case DbType.SByte: return "Byte";
                case DbType.Single: return "Single";
                case DbType.String: return "String";
                case DbType.StringFixedLength: return "String";
                case DbType.Time: return "TimeSpan";
                //case DbType.UInt16: return "UInt16"; // siegmar
                case DbType.UInt16: return "Int16";
                //case DbType.UInt32: return "UInt32"; // siegmar
                case DbType.UInt32: return "Int32";
                //case DbType.UInt64: return "UInt64"; // siegmar
                case DbType.UInt64: return "Int64";
                case DbType.VarNumeric: return "VarNumeric";
                //case DbType.Xml: return "Xml";
                //case DbType.Xml: return "String"; // 2.6
                //default: return "_" + column.NativeType + "_";
                default: return "String";
            }
        }        
        
        public string GetLINQToDBType(DbType dbType)
        {
            switch (dbType)
            {
                case DbType.AnsiString: return "VarChar";
                case DbType.AnsiStringFixedLength: return "VarChar";
                case DbType.Binary: return "Binary";
                case DbType.Boolean: return "Boolean";
                case DbType.Byte: return "Byte";
                //case DbType.Currency: return "Currency";
                case DbType.Currency: return "Decimal";
                case DbType.Date: return "DateTime";
                case DbType.DateTime: return "DateTime";
                //case DbType.DateTime2: return ""; // 2.6
                //case DbType.DateTimeOffset: return ""; // 2.6
                case DbType.Decimal: return "Decimal";
                case DbType.Double: return "Double";
                case DbType.Guid: return "Guid";
                case DbType.Int16: return "Int16";
                case DbType.Int32: return "Int32";
                case DbType.Int64: return "Int64";
                //case DbType.Object: return "Object";
                case DbType.Object: return "Binary";
                //case DbType.SByte: return "SByte"; // siegmar
                case DbType.SByte: return "Byte";
                case DbType.Single: return "Single";
                case DbType.String: return "VarChar";
                case DbType.StringFixedLength: return "VarChar";
                case DbType.Time: return "Time";
                //case DbType.UInt16: return "UInt16"; // siegmar
                case DbType.UInt16: return "Int16";
                //case DbType.UInt32: return "UInt32"; // siegmar
                case DbType.UInt32: return "Int32";
                //case DbType.UInt64: return "UInt64"; // siegmar
                case DbType.UInt64: return "Int64";
                case DbType.VarNumeric: return "VarNumeric";
                //case DbType.Xml: return "Xml";
                //case DbType.Xml: return "String"; // 2.6
                //default: return "_" + column.NativeType + "_";
                default: return "String";
            }
        }

        public string GetSqlType(DbType dbType)
        {
            switch (dbType)
            {
                case DbType.AnsiString: return "varchar";
                case DbType.AnsiStringFixedLength: return "varchar";
                case DbType.Binary: return "varbinary";
                case DbType.Boolean: return "bit";
                case DbType.Byte: return "tinyint";
                case DbType.Currency: return "money";
                case DbType.Date: return "date";
                case DbType.DateTime: return "datetime";
                case DbType.Decimal: return "decimal";
                case DbType.Double: return "float";
                case DbType.Guid: return "uniqueidentifier";
                case DbType.Int16: return "smallint";
                case DbType.Int32: return "int";
                case DbType.Int64: return "bigint";
                case DbType.Object: return "binary";
                case DbType.SByte: return "tinyint";
                case DbType.Single: return "real";
                case DbType.String: return "varchar";
                case DbType.StringFixedLength: return "varchar";
                case DbType.Time: return "time"; // TimeSpan => time
                case DbType.UInt16: return "smallint";
                case DbType.UInt32: return "int";
                case DbType.UInt64: return "bigint";
                case DbType.VarNumeric: return "decimal";
                default: return "varchar";
            }

            return dbType.ToString();
        }        

        public string GetType(DbType dbType, bool isNullable)
        {
            if (isNullable)
            {
                return GetTypeNullable(dbType);
            }
            else
            {
                return GetType(dbType);
            }
        }

        public string GetType(DbType dbType)
        {
            switch (dbType)
            {
                case DbType.AnsiString: return "string";
                case DbType.AnsiStringFixedLength: return "string";
                case DbType.Binary: return "byte[]";
                case DbType.Boolean: return "bool";
                case DbType.Byte: return "byte";
                case DbType.Currency: return "decimal";
                case DbType.Date: return "DateTime";
                case DbType.DateTime: return "DateTime";
                //case DbType.DateTime2: return ""; // 2.6
                //case DbType.DateTimeOffset: return ""; // 2.6
                case DbType.Decimal: return "decimal";
                case DbType.Double: return "double";
                case DbType.Guid: return "Guid";
                case DbType.Int16: return "short";
                case DbType.Int32: return "int";
                case DbType.Int64: return "long";
                //case DbType.Object: return "object";
                case DbType.Object: return "byte[]";
                //case DbType.SByte: return "sbyte"; // siegmar
                case DbType.SByte: return "byte";
                case DbType.Single: return "float";
                case DbType.String: return "string";
                case DbType.StringFixedLength: return "string";
                case DbType.Time: return "TimeSpan";
                //case DbType.UInt16: return "ushort"; // siegmar
                case DbType.UInt16: return "short";
                //case DbType.UInt32: return "uint"; // siegmar
                case DbType.UInt32: return "int";
                //case DbType.UInt64: return "ulong"; // siegmar
                case DbType.UInt64: return "long";
                case DbType.VarNumeric: return "decimal";
                //case DbType.Xml: return "xml";
                //case DbType.Xml: return "string"; // 2.6
                //default: return "_" + column.NativeType + "_";
                default: return "string";
            }
        }
        
        public string GetTypeNullable(DbType dbType)
        {
            switch (dbType)
            {
                case DbType.AnsiString: return "string";
                case DbType.AnsiStringFixedLength: return "string";
                case DbType.Binary: return "byte[]";
                case DbType.Boolean: return "bool?";
                case DbType.Byte: return "byte?";
                case DbType.Currency: return "decimal?";
                case DbType.Date: return "DateTime?";
                case DbType.DateTime: return "DateTime?";
                //case DbType.DateTime2: return ""; // 2.6
                //case DbType.DateTimeOffset: return ""; // 2.6
                case DbType.Decimal: return "decimal?";
                case DbType.Double: return "double?";
                case DbType.Guid: return "Guid?";
                case DbType.Int16: return "short?";
                case DbType.Int32: return "int?";
                case DbType.Int64: return "long?";
                //case DbType.Object: return "object";
                case DbType.Object: return "byte[]";
                //case DbType.SByte: return "sbyte"; // siegmar
                case DbType.SByte: return "byte?";
                case DbType.Single: return "float?";
                case DbType.String: return "string";
                case DbType.StringFixedLength: return "string";
                case DbType.Time: return "TimeSpan?";
                //case DbType.UInt16: return "ushort?";
                case DbType.UInt16: return "short?";
                //case DbType.UInt32: return "uint?";
                case DbType.UInt32: return "int?";
                //case DbType.UInt64: return "ulong?";
                case DbType.UInt64: return "long?";
                case DbType.VarNumeric: return "decimal?";
                //case DbType.Xml: return "Xml?";
                //case DbType.Xml: return "string"; // 2.6
                //default: return "_" + column.NativeType + "_";
                //default: return "_" + dbType.ToString() + "_";
                default: return "string";
            }
        }        
        
        #endregion
        
        #region Schema
        
        // PK

        //public string PKColumnsSQL(MemberColumnSchemaCollection columns) // 2.6
        public string PKColumnsSQL(ColumnSchemaCollection columns)
        {
            string result = "";
            
            //for (int i = 0;i < columns.Count;i++)
            foreach (ColumnSchema column in columns)
            {
                if (!result.Equals(""))
                {
                    result += ",";
                }
                //result += columns[i].Name;    
                result += column.Name;    
            }
            
            return result;
        }

        public string GetDataToString(ColumnSchema column, string prefix, bool isProperty)
        {
            string name = isProperty ? PropertyName(column.Name) : LocalName(column.Name);
            string result = "";           
            
            //if (IsSysColumn(column))
            //{
            //    result = "LibraryHelper.DataToString(" + prefix + name + ", ResourceHelper.Format_SysColumn)";
            //}
            if (IsDate(column.DataType))
            {
                result = "LibraryHelper.DataToString(" + prefix + name + ", ResourceHelper.Format_Date)";
            }
            else if (IsDateTime(column.DataType))
            {
                result = "LibraryHelper.DataToString(" + prefix + name + ", ResourceHelper.Format_DateTime)";
            }
            else if (IsBinary(column.DataType) || IsBoolean(column.DataType) || IsGuid(column.DataType) || IsString(column.DataType))
            {
                result = "LibraryHelper.DataToString(" + prefix + name + ")";
            }
            else
            {
                result = "LibraryHelper.DataToString(" + prefix + name + ", ResourceHelper." + (column.DataType) + ")";
            }
            
            return result;
        }

        //public string PKColumnsSQLParameters(MemberColumnSchemaCollection columns) // 2.6
        public string PKColumnsSQLParameters(ColumnSchemaCollection columns)
        {
            string result = "";
            
            for (int i = 0;i < columns.Count;i++)
            {
                if (!result.Equals(""))
                {
                    result += ",";
                }
                result += "#" + columns[i].Name;    
            }
            
            return result;
        }
        
        // FK
                
        public string FKColumnsSQL(TableKeySchema table, string suffix)
        {
            string result = "";

            for (int i = 0;i < table.ForeignKeyMemberColumns.Count;i++)
            {
                if (!result.Equals(""))
                {
                    result += " AND ";
                }
                result += TableName(table.PrimaryKeyTable.Name) + suffix + "." + table.PrimaryKeyMemberColumns[i].Name +
                    " = " + TableName(table.ForeignKeyTable.Name) + "." + table.ForeignKeyMemberColumns[i].Name;
            }

            return result;
        }

        public int FKIndexOf(TableKeySchemaCollection foreignKeys, string fkColumn) // 2.6
        {
            int i = 0, index = -1;
            
            foreach (TableKeySchema fkTable in foreignKeys)
            {
                if (fkTable.ForeignKeyMemberColumns[0].Name == fkColumn)
                {
                    index = i;
                }
                i++;
            }
            
            return index;
        }
        
        public string FKTableName(TableSchema table, ColumnSchema column)
        {
            string fkTableName = "";
            
            foreach (TableKeySchema tableX in table.ForeignKeys)
            {
                foreach (ColumnSchema columnX in tableX.ForeignKeyMemberColumns)
                {
                    if (columnX.Name == column.Name)
                    {
                        fkTableName = tableX.PrimaryKeyTable.FullName;
                        break;
                    }
                }
            }
            
            return fkTableName;
        }

        // 
        
        public string ColumnFK(ColumnSchema column)
        {
            if (column.IsForeignKeyMember)
            {
                return "FK";
            }
            else
            {
                return "--";
            }
        }

        public string ColumnPK(ColumnSchema column)
        {
            if (column.IsPrimaryKeyMember)
            {
                return "PK";
            }
            else
            {
                return "--";
            }
        }
        
        public string ColumnNULL(ColumnSchema column)
        {
            if (column.AllowDBNull)
            {
                return "NULL";
            }
            else
            {
                return "----";
            }
        }
        
        #endregion
        
        #region Schema Extended Properties
        
        public bool IsIdentity(ColumnSchema column)
        {
            bool result = false;
            
            if (column.ExtendedProperties["CS_IsIdentity"] != null) // 2.6
            {
                result = (bool)column.ExtendedProperties["CS_IsIdentity"].Value;
            }
            
            return result;
        }
        
        public bool IsImage(ColumnSchema column)
        {
            bool result = false;
            
            if (column.ExtendedProperties["CS_SystemType"] != null) // 2.6
            {
                result = (((string)(column.ExtendedProperties["CS_SystemType"].Value)).ToLower() == "image");
            }
            
            return result;
        }
        
        public bool IsNText(ColumnSchema column)
        {
            bool result = false;
            
            if (column.ExtendedProperties["CS_SystemType"] != null) // 2.6
            {
                result = (((string)(column.ExtendedProperties["CS_SystemType"].Value)).ToLower() == "ntext");
            }
            
            return result;
        }

        #endregion
        
        #region Width
        
        public int TextBoxWidth(int width)
        {
            if (width <= 1)
                return 10;
            else if (width <= 2)
                return 20;
            else if (width <= 3)
                return 30;
            else if (width <= 4)
                return 40;
            else if (width <= 5)
                return 50;
            else if (width <= 10)
                return 80;
            else if (width <= 20)
                return 160;
            else if (width <= 30)
                return 240;
            else if (width <= 40)
                return 320;
            else
                return 400;
        }
        
        public string EditWidth(ColumnSchema column)
        {
            string result;            
            
            if (IsString(column.DataType))
            {
                if (column.Size <= 5)
                {                
                    //result = "col-md-1";
                    result = "col-md-2";
                }
                else if (column.Size <= 15)
                {                
                    result = "col-md-2";
                }
                else if (column.Size <= 25)
                {                
                    result = "col-md-3";
                }
                else if (column.Size <= 38)
                {                
                    result = "col-md-4";
                }
                else if (column.Size <= 50)
                {                
                    result = "col-md-5";
                }
                else
                {                
                    result = "col-md-6";
                }
            }
            else if (IsDate(column.DataType))
            {
                //result = "col-md-1";
                result = "col-md-2";
            }
            else if (IsDateTime(column.DataType))
            {
                result = "col-md-2";
            }
            else if (IsDecimal(column.DataType) || IsFloat(column.DataType))
            {
                //result = "col-md-1";
                result = "col-md-2";
            }
            else if (IsInteger(column.DataType))        
            {
                //result = "col-md-1";
                result = "col-md-2";
            }
            else
            {
                result = "col-md-2";
            }
            
            return result;
        }
        
        public string IndexWidth(ColumnSchema column)
        {
            int result;            
            int factor = 10;
            
            if (IsString(column.DataType))
            {
                result = column.Size * factor;
                result = result > 240 ? 240 : result;
            }
            else if (IsDate(column.DataType))
            {
                result = 20 * factor;
            }
            else if (IsDate(column.DataType))
            {
                result = 23 * factor;
            }
            else if (IsDecimal(column.DataType) || IsFloat(column.DataType))
            {
                result = 10 * factor;
            }
            else if (IsInteger(column.DataType))        
            {
                result = 10 * factor;
            }
            else
            {
                result = 10 * factor;
            }
            
            return result.ToString() + "px";
        } 
        
        #endregion        
    }
}

