using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;




namespace ElecStoreDataAccess
{

    public static class CLSCategoryData
    {

        public static bool GetCategoryByID(int categoryID, ref string categoryName, ref string cDescription)
        {

            bool isFound = false;


            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {

                using (SqlCommand command = new SqlCommand("sp_Categories_GetByID", connection))
                {

                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@CategoryID", categoryID);


                    try
                    {

                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            if (reader.Read())
                            {

                                isFound = true;

                                categoryName = (string)reader["CategoryName"];


                                if (reader["CDescription"] != DBNull.Value)

                                    cDescription = (string)reader["CDescription"];
                                else
                                    cDescription = "";

                            }

                        }

                    }
                    catch (Exception)
                    {

                        isFound = false;

                    }

                }

            }

            return isFound;

        }


        public static bool IsCategoryExist(int categoryID)
        {

            bool isFound = false;


            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {

                string query = "SELECT Found = 1 FROM Categories WHERE CategoryID = @CategoryID";


                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    command.Parameters.AddWithValue("@CategoryID", categoryID);

                    try
                    {

                        connection.Open();

                        object result = command.ExecuteScalar();


                        if (result != null)
                        {

                            isFound = true;

                        }

                    }

                    catch (Exception ex)
                    {

                        // Handle or log exception

                        isFound = false;

                    }

                }

            }

            return isFound;

        }



        public static int AddNewCategory(string categoryName, string cDescription)
        {

            int categoryID = -1;


            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {

                using (SqlCommand command = new SqlCommand("sp_Categories_Insert", connection))
                {

                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@CategoryName", categoryName);


                    if (string.IsNullOrEmpty(cDescription))
                        command.Parameters.AddWithValue("@CDescription", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@CDescription", cDescription);


                    SqlParameter outputIDParam = new SqlParameter("@NewCategoryID", SqlDbType.Int){

                        Direction = ParameterDirection.Output

                    };

                    command.Parameters.Add(outputIDParam);


                    try
                    {

                        connection.Open();

                        command.ExecuteNonQuery();

                        if (outputIDParam.Value != DBNull.Value)
                        {

                            categoryID = (int)outputIDParam.Value;

                        }

                    }

                    catch (Exception ex)
                    {

                        // يمكن تسجيل الخطأ هنا عند الحاجة

                    }

                }

            }

            return categoryID;

        }


        public static bool UpdateCategory(int categoryID, string categoryName, string cDescription)
        {

            int rowsAffected = 0;


            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {

                using (SqlCommand command = new SqlCommand("sp_Categories_Update", connection))
                {

                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@CategoryID", categoryID);

                    command.Parameters.AddWithValue("@CategoryName", categoryName);


                    if (string.IsNullOrEmpty(cDescription))

                        command.Parameters.AddWithValue("@CDescription", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@CDescription", cDescription);


                    try
                    {

                        connection.Open();

                        rowsAffected = command.ExecuteNonQuery();

                    }

                    catch (Exception)
                    {

                        return false;

                    }

                }

            }

            return (rowsAffected > 0);

        }


        public static bool DeleteCategory(int categoryID)
        {

            int rowsAffected = 0;


            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {

                using (SqlCommand command = new SqlCommand("sp_Categories_Delete", connection))
                {

                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@CategoryID", categoryID);


                    try
                    {

                        connection.Open();

                        rowsAffected = command.ExecuteNonQuery();

                    }

                    catch (Exception)
                    {

                        return false;

                    }

                }

            }

            return (rowsAffected > 0);

        }




        public static DataTable GetAllCategories()
        {

            DataTable dt = new DataTable();


            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {

                using (SqlCommand command = new SqlCommand("sp_Categories_GetAll", connection))
                {

                    command.CommandType = CommandType.StoredProcedure;

                    try
                    {

                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            if (reader.HasRows)
                            {

                                dt.Load(reader);

                            }

                        }

                    }

                    catch (Exception)
                    {

                        // تعيد DataTable فارغ في حال وجود خطأ

                    }

                }

            }


            return dt;

        }


    }


}




















