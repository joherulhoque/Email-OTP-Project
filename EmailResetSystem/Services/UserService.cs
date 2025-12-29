using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ResetSystem.Services
{
    public class UserService
    {
        private readonly string _conn;

        public UserService(IConfiguration config)
        {
            _conn = config.GetConnectionString("Default");
        }

        public bool UserExists(string username)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Username=@u", con);
            cmd.Parameters.AddWithValue("@u", username);
            con.Open();
            return (int)cmd.ExecuteScalar()! > 0;
        }

        public string? GetMobileByUsername(string username)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SELECT Mobile FROM Users WHERE Username=@u", con);
            cmd.Parameters.AddWithValue("@u", username);
            con.Open();
            return cmd.ExecuteScalar()?.ToString();
        }

        public (string Username, string Email)? GetUserByUsernameOrEmail(string usernameOrEmail)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SELECT Username, Email FROM Users WHERE Username=@u OR Email=@u", con);
            cmd.Parameters.AddWithValue("@u", usernameOrEmail);
            con.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return (reader.GetString(0), reader.GetString(1));
            }
            return null;
        }
        public void SaveOtp(string username, string otp)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "INSERT INTO OTPStore (Username, OTP, ExpireTime, Used) VALUES (@u,@o,DATEADD(MINUTE,5,GETDATE()),0)", con);
            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@o", otp);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        public bool VerifyOtp(string username, string otp)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "SELECT Id FROM OTPStore WHERE Username=@u AND OTP=@o AND Used=0 AND ExpireTime>GETDATE()", con);
            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@o", otp);
            con.Open();
            return cmd.ExecuteScalar() != null;
        }

        public void MarkOtpUsed(string username, string otp)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "UPDATE OTPStore SET Used=1 WHERE Username=@u AND OTP=@o", con);
            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@o", otp);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        public void UpdatePassword(string username, string passwordHash)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("UPDATE Users SET PasswordHash=@p WHERE Username=@u", con);
            cmd.Parameters.AddWithValue("@p", passwordHash);
            cmd.Parameters.AddWithValue("@u", username);
            con.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
