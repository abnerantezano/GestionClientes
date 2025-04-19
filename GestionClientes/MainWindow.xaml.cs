using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Data.SqlClient;

namespace GestionClientes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string connectionString = "Server=DESKTOP-J9ERERR\\SQLEXPRESS;Database=Neptuno;User Id=user01;Password=12345;TrustServerCertificate=True;";
        public MainWindow()
        {
            InitializeComponent();
            CargarClientes();
        }

        private void CargarClientes()
        {
            List<Cliente> clientes = new();

            using SqlConnection connection = new(connectionString);
            connection.Open();

            SqlCommand command = new("SELECT * FROM clientes WHERE Activo = 1", connection);
            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                clientes.Add(new Cliente
                {
                    IdCliente = reader["idCliente"].ToString(),
                    NombreCompañia = reader["NombreCompañia"].ToString(),
                    NombreContacto = reader["NombreContacto"].ToString(),
                    CargoContacto = reader["CargoContacto"].ToString(),
                    Direccion = reader["Direccion"].ToString(),
                    Ciudad = reader["Ciudad"].ToString(),
                    Region = reader["Region"].ToString(),
                    CodPostal = reader["CodPostal"].ToString(),
                    Pais = reader["Pais"].ToString(),
                    Telefono = reader["Telefono"].ToString(),
                    Fax = reader["Fax"].ToString(),
                    Activo = true
                });
            }

            ClientesDataGrid.ItemsSource = clientes;
        }

        private void InsertarCliente_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();

                SqlCommand command = new SqlCommand("sp_InsertarCliente", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@IdCliente", "C002");
                command.Parameters.AddWithValue("@NombreCompañia", "Nueva Compañía SAC");
                command.Parameters.AddWithValue("@NombreContacto", "Carlos Paredes");
                command.Parameters.AddWithValue("@CargoContacto", "Supervisor");
                command.Parameters.AddWithValue("@Direccion", "Av. Perú 123");
                command.Parameters.AddWithValue("@Ciudad", "Lima");
                command.Parameters.AddWithValue("@Region", "Lima");
                command.Parameters.AddWithValue("@CodPostal", "15001");
                command.Parameters.AddWithValue("@Pais", "Perú");
                command.Parameters.AddWithValue("@Telefono", "987654321");
                command.Parameters.AddWithValue("@Fax", DBNull.Value);
                command.Parameters.AddWithValue("@Activo", 1);

                command.ExecuteNonQuery();

                MessageBox.Show("Cliente insertado.");
                CargarClientes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al insertar: " + ex.Message);
            }
        }
    }
}