using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using Microsoft.Data.SqlClient;

namespace GestionClientes
{
    public partial class MainWindow : Window
    {
        private readonly string connectionString = "Server=DESKTOP-J9ERERR\\SQLEXPRESS;Database=Neptuno;User Id=user01;Password=123456;TrustServerCertificate=True;";
        private int paginaActual = 1;
        private const int tamanioPagina = 20;

        public MainWindow()
        {
            InitializeComponent();
            CargarClientes();
        }

        private void CargarClientes()
        {
            List<Cliente> clientes = new List<Cliente>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("sp_ListarClientes", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Pagina", paginaActual);
                command.Parameters.AddWithValue("@TamanoPagina", tamanioPagina);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        clientes.Add(new Cliente
                        {
                            IdCliente = reader["IdCliente"].ToString(),
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
                            Activo = Convert.ToBoolean(reader["Activo"])
                        });
                    }
                }
            }

            dgClientes.ItemsSource = clientes;
            txtPagina.Text = $"Página {paginaActual}";
        }

        private void BtnAnterior_Click(object sender, RoutedEventArgs e)
        {
            if (paginaActual > 1)
            {
                paginaActual--;
                CargarClientes();
            }
        }

        private void BtnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            paginaActual++;
            CargarClientes();
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            ClienteForm form = new ClienteForm(FormMode.Add);
            if (form.ShowDialog() == true)
                CargarClientes();
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (dgClientes.SelectedItem is Cliente clienteSeleccionado)
            {
                ClienteForm form = new ClienteForm(FormMode.Edit, clienteSeleccionado);
                if (form.ShowDialog() == true)
                    CargarClientes();
            }
            else
            {
                MessageBox.Show("Seleccione un cliente para editar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            string idCliente = txtIdEliminar.Text.Trim();

            if (string.IsNullOrWhiteSpace(idCliente))
            {
                MessageBox.Show("Por favor, ingrese un ID de cliente válido para eliminar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var resultado = MessageBox.Show($"¿Está seguro que desea eliminar al cliente con ID '{idCliente}'?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (resultado != MessageBoxResult.Yes)
                return;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand("sp_EliminarCliente", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdCliente", idCliente);

                    connection.Open();
                    int filasAfectadas = command.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        MessageBox.Show("Cliente eliminado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        txtIdEliminar.Clear();
                        CargarClientes();
                    }
                    else
                    {
                        MessageBox.Show("No se encontró un cliente con ese ID.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar cliente: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
