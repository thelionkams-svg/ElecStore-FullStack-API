using ElecStoreDataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;




namespace ElecStoreBusiness
{

    public class CLSCategory
    {

        public enum enMode { AddNew = 0, Update = 1 }

        public enMode Mode = enMode.AddNew;


        public int CategoryID { get; set; }

        public string CategoryName { get; set; }

        public string CDescription { get; set; }




        public CLSCategory()
        {

            this.CategoryID = -1;

            this.CategoryName = "";

            this.CDescription = "";


            Mode = enMode.AddNew;

        }

        private CLSCategory(int categoryID, string categoryName, string cDescription)
        {

            this.CategoryID = categoryID;

            this.CategoryName = categoryName;

            this.CDescription = cDescription;


            Mode = enMode.Update;

        }




        public static CLSCategory Find(int categoryID)
        {

            string categoryName = "";

            string cDescription = "";


            if (CLSCategoryData.GetCategoryByID(categoryID, ref categoryName, ref cDescription))
            {

                return new CLSCategory(categoryID, categoryName, cDescription);

            }

            else
            {

                return null;

            }

        }


        public static bool IsCategoryExist(int categoryID)
        {

            return CLSCategoryData.IsCategoryExist(categoryID);

        }



        private bool _AddNewCategory()
        {

            this.CategoryID = CLSCategoryData.AddNewCategory(this.CategoryName, this.CDescription);

            return (this.CategoryID != -1);

        }

        private bool _UpdateCategory()
        {

            return CLSCategoryData.UpdateCategory(this.CategoryID, this.CategoryName, this.CDescription);
        
        }

        public bool Save()
        {

            switch (Mode)
            {

                case enMode.AddNew:

                    if (_AddNewCategory())
                    {

                        Mode = enMode.Update; 
                      
                        return true;
                   
                    }
                   
                    else
                    {

                        return false;

                    }

                case enMode.Update:

                    return _UpdateCategory();

            }


            return false;

        }



        public static bool DeleteCategory(int categoryID)
        {

            return CLSCategoryData.DeleteCategory(categoryID);
       
        }

        public static DataTable GetAllCategories()
        {

            return CLSCategoryData.GetAllCategories();

        }


    }

}
