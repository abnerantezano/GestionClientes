using System;
using System.Windows;
using Microsoft.Data.SqlClient;
using System.Data;

namespace GestionClientes
{
    public enum FormMode
    {
        Add,
        Edit
    }

    public partial class ClienteForm : Window
    {
        private readonly string connectionString = "Server=DESKTOP-J9ERERR\\SQLEXPRESS;Database=Neptuno;User Id=user01;Password=123456;TrustServerCertificate=True;";
        public FormMode Mode { get; set; }
        public Cliente ClienteSeleccionado { get; set; }

        public ClienteForm(FormMode mode, Cliente cliente = null)
        {
            InitializeComponent();
            Mode = mode;
            ClienteSeleccionado = cliente;

            if (Mode == FormMode.Edit && cliente != null)
                CargarDatos(cliente);

            if (Mode == FormMode.Add)
                Title = "Agregar Cliente";
            else if (Mode == FormMode.Edit)
                Title = "Editar Cliente";
        }

        private void CargarDatos(Cliente cliente)
        {
            txtIdCliente.Text = cliente.IdCliente;
            txtNombreCompañia.Text = cliente.NombreCompañia;
            txtNombreContacto.Text = cliente.NombreContacto;
            txtCargoContacto.Text = cliente.CargoContacto;
            txtDireccion.Text = cliente.Direccion;
            txtCiudad.Text = cliente.Ciudad;
            txtRegion.Text = cliente.Region;
            txtCodPostal.Text = cliente.CodPostal;
            txtPais.Text = cliente.Pais;
            txtTelefono.Text = cliente.Telefono;
            txtFax.Text = cliente.Fax;
            chkActivo.IsChecked = cliente.Activo;

            // Bloquear el ID al editar
            txtIdCliente.IsReadOnly = true;
        }

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            if (Mode == FormMode.Add)
                InsertarCliente();
            else if (Mode == FormMode.Edit)
                ActualizarCliente();

            DialogResult = true;
            Close();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void InsertarCliente()
        {
            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand("sp_InsertarCliente", connection);
            command.CommandType = CommandType.StoredProcedure;

            AgregarParametros(command);
            connection.Open();
            command.ExecuteNonQuery();
        }

        private void ActualizarCliente()
        {
            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand("sp_ActualizarCliente", connection);
            command.CommandType = CommandType.StoredProcedure;

            AgregarParametros(command);
            connection.Open();
            command.ExecuteNonQuery();
        }

        private void AgregarParametros(SqlCommand command)
        {
            command.Parameters.AddWithValue("@IdCliente", txtIdCliente.Text);
            command.Parameters.AddWithValue("@NombreCompañia", txtNombreCompañia.Text);
            command.Parameters.AddWithValue("@NombreContacto", (object?)txtNombreContacto.Text ?? DBNull.Value);
            command.Parameters.AddWithValue("@CargoContacto", (object?)txtCargoContacto.Text ?? DBNull.Value);
            command.Parameters.AddWithValue("@Direccion", (object?)txtDireccion.Text ?? DBNull.Value);
            command.Parameters.AddWithValue("@Ciudad", (object?)txtCiudad.Text ?? DBNull.Value);
            command.Parameters.AddWithValue("@Region", (object?)txtRegion.Text ?? DBNull.Value);
            command.Parameters.AddWithValue("@CodPostal", (object?)txtCodPostal.Text ?? DBNull.Value);
            command.Parameters.AddWithValue("@Pais", (object?)txtPais.Text ?? DBNull.Value);
            command.Parameters.AddWithValue("@Telefono", (object?)txtTelefono.Text ?? DBNull.Value);
            command.Parameters.AddWithValue("@Fax", (object?)txtFax.Text ?? DBNull.Value);
            command.Parameters.AddWithValue("@Activo", chkActivo.IsChecked == true ? 1 : 0);
        }
    }
}
