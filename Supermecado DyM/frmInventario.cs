using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Supermecado_DyM
{
    public partial class frmInventario : Form
    {
        public frmInventario()
        {
            InitializeComponent();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            Convertir_Porcentaje();
        }

        private void Ingresar_Producto(decimal IVA, decimal Descuento)
        {
            try
            {
                Conexion Conn =  new Conexion();
                Conn.LeerCadena();

                SqlCommand ComandoProductos = new SqlCommand("SP_INGRESAR_PRODUCTO", Conn.LeerCadena());
                ComandoProductos.CommandType = CommandType.StoredProcedure;
                ComandoProductos.Parameters.AddWithValue("@CODIGO", txtCodigo.Text.Trim());
                ComandoProductos.Parameters.AddWithValue("@CLASIFICACION", cmbClasificacion.Text.Trim());
                ComandoProductos.Parameters.AddWithValue("@DESCPRODUCTO", txtProducto.Text.Trim());
                ComandoProductos.Parameters.AddWithValue("@CANTIDAD", txtCantidad.Text.Trim());
                ComandoProductos.Parameters.AddWithValue("@PRECIO", txtPrecio.Text.Trim());
                ComandoProductos.Parameters.AddWithValue("@IVA", IVA);
                ComandoProductos.Parameters.AddWithValue("@DESCUENTO", Descuento);

                ComandoProductos.ExecuteNonQuery();

                MessageBox.Show("Se ha ingresado el producto satisfactoriamente", "Mensaje Productos",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                Conn.LeerCadena().Dispose();

                LLenar_DataGrid_Productos();
            }
            catch (Exception Ex)
            {

                MessageBox.Show(Ex.Message);
            }
                       
        }

        private void LLenar_DataGrid_Productos()
        {
            try
            {
                Conexion Conn = new Conexion();
                Conn.LeerCadena();

                SqlCommand ComandoProductos = new SqlCommand("SP_LLENAR_PRODUCTOS", Conn.LeerCadena());
                ComandoProductos.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter Productos = new SqlDataAdapter(ComandoProductos);
                DataTable DTProductos = new DataTable();

                Productos.Fill(DTProductos);
                dgvProductos.DataSource = DTProductos;

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void Convertir_Porcentaje() 
        {
            decimal IVA;
            decimal Descuento;


           IVA = Convert.ToDecimal(txtPorIVA.Text.Trim()) / 100;
           Descuento = Convert.ToDecimal(txtPorDescuento.Text.Trim()) / 100;

          Ingresar_Producto(IVA, Descuento);
           
        }

        private void frmInventario_Load(object sender, EventArgs e)
        {
            LLenar_DataGrid_Productos();
        }

        private void dgvProductos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                int cantidadfilas = dgvProductos.Rows.Count;

                for (int i = 0; i < cantidadfilas; i++)
                {
                    if (dgvProductos.Rows[i].Cells[3].Value.ToString() == "CARNES")
                    {
                      //  dgvProductos.Rows[i].DefaultCellStyle.BackColor = Color.Green;
                    }

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private void btnModificar_Click(object sender, EventArgs e)
        {


            Convertir_Porcentaje_Modificado();


         //   LLenar_DataGrid_Productos();
        }

        // ------------------------------------------------------------------------------------//
        // ---------------------------- Begin Modificar ---------------------------------------//
        // ------------------------------------------------------------------------------------//

        private void Modificar_Producto(decimal IVA, decimal Descuento)
        {
            try
            {
                Conexion Conn = new Conexion();
                Conn.LeerCadena();

                SqlCommand ComandoProductos = new SqlCommand("SP_MODIFICAR_PRODUCTO", Conn.LeerCadena());
                ComandoProductos.CommandType = CommandType.StoredProcedure;
               // ComandoProductos.Parameters.AddWithValue("@ID_PRODUCTOS", txtID_Producto.Text.Trim());
                ComandoProductos.Parameters.AddWithValue("@CODIGO", txtCodigo.Text.Trim());
                ComandoProductos.Parameters.AddWithValue("@CLASIFICACION", cmbClasificacion.Text.Trim());
                ComandoProductos.Parameters.AddWithValue("@DESCPRODUCTO", txtProducto.Text.Trim());
                ComandoProductos.Parameters.AddWithValue("@CANTIDAD", txtCantidad.Text.Trim());
                ComandoProductos.Parameters.AddWithValue("@PRECIO", txtPrecio.Text.Trim());
                ComandoProductos.Parameters.AddWithValue("@IVA", IVA);
                ComandoProductos.Parameters.AddWithValue("@DESCUENTO", Descuento);

                ComandoProductos.ExecuteNonQuery();

                MessageBox.Show("Se ha modificado el producto satisfactoriamente", "Mensaje Productos",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                Conn.LeerCadena().Dispose();

                LLenar_DataGrid_Productos();
            }
            catch (Exception Ex)
            {

                MessageBox.Show(Ex.Message);
            }

        }

        private void Convertir_Porcentaje_Modificado()
        {
            decimal IVA;
            decimal Descuento;


            IVA = Convert.ToDecimal(txtPorIVA.Text.Trim()) / 100;
            Descuento = Convert.ToDecimal(txtPorDescuento.Text.Trim()) / 100;

            Modificar_Producto(IVA, Descuento);

        }


        // ------------------------------------------------------------------------------------//
        // -----------------------------  End Modificar  --------------------------------------//
        // ------------------------------------------------------------------------------------//



        // ------------------------------------------------------------------------------------//
        // ----------------------------   Modificado Mas Variables  ---------------------------//
        // ------------------------------------------------------------------------------------//

        private void dgvProductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            try
                {
                    DataGridViewRow fila = dgvProductos.Rows[e.RowIndex];
                    decimal IVA;
                    IVA = Convert.ToDecimal(dgvProductos.SelectedCells[6].Value.ToString());
                    decimal IVA_ENTERO = IVA * 100;

                    decimal Descuento;
                    Descuento = Convert.ToDecimal(dgvProductos.SelectedCells[7].Value.ToString());
                    decimal DESCUENTO_ENTERO = Descuento * 100;

                    txtCodigo.Text = dgvProductos.SelectedCells[1].Value.ToString();
                    txtProducto.Text = dgvProductos.SelectedCells[2].Value.ToString();
                    txtCantidad.Text = dgvProductos.SelectedCells[4].Value.ToString();
                    txtPrecio.Text = dgvProductos.SelectedCells[5].Value.ToString();
                    cmbClasificacion.Text = dgvProductos.SelectedCells[3].Value.ToString();
                    txtPorIVA.Text = IVA_ENTERO.ToString("0.##");
                    txtPorDescuento.Text = DESCUENTO_ENTERO.ToString("0.##");

                    Calcular_Total();

                }
            catch
                {
                    MessageBox.Show("Columna seleccionada no permitida, por favor seleccione la Columna numero 1");
                }

        }

        // ------------------------------------------------------------------------------------//
        // ----------------------------   Modificado    ---------------------------------------//
        // ------------------------------------------------------------------------------------//

        private void Calcular_Total()
        {
            int total;
            int Cantidad;
            int Precio;

            Cantidad = Convert.ToInt32(txtCantidad.Text);
            Precio = Convert.ToInt32(txtPrecio.Text);
            total = Cantidad * Precio;

            lblTotal.Text = total.ToString();
        }

        private void txtCantidad_TextChanged(object sender, EventArgs e)
        {
            //Calcular_Total();
        }

        private void txtPrecio_TextChanged(object sender, EventArgs e)
        {
            //Calcular_Total();
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            Consultar_Producto();
        }
        private void Consultar_Producto()
        {
            try
            {
                Conexion Conn = new Conexion();
                Conn.LeerCadena();

                SqlCommand ComandoProducto = new SqlCommand("SP_CONSULTA_PRODUCTO_NOMBRE", Conn.LeerCadena());
                DataTable tabla = new DataTable();

                ComandoProducto.CommandType = CommandType.StoredProcedure;
                ComandoProducto.Parameters.AddWithValue("@NOMBREPRODUCTO", txtBuscar.Text);

                SqlDataReader ConsultaProducto = ComandoProducto.ExecuteReader();


                if (ConsultaProducto.Read())
                {

                    LLenar_DataGrid_Productos_Busqueda();

                    Conn.LeerCadena();

                }

                else
                {
                    //Limpiar_Campos();
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Mensaje Importante", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            }
        }

        private void LLenar_DataGrid_Productos_Busqueda()
        {
            try
            {
                Conexion Conn = new Conexion();
                Conn.LeerCadena();

                SqlCommand ComandoProductos = new SqlCommand("SP_LLENAR_PRODUCTOS_BUSQUEDA", Conn.LeerCadena());
                ComandoProductos.CommandType = CommandType.StoredProcedure;
                ComandoProductos.Parameters.AddWithValue("@NOMBREPRODUCTO", txtBuscar.Text);

                SqlDataAdapter Productos = new SqlDataAdapter(ComandoProductos);
                DataTable DTProductos = new DataTable();

                Productos.Fill(DTProductos);
                dgvProductos.DataSource = DTProductos;

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void txtProducto_TextChanged(object sender, EventArgs e)
        {

        }



        // ------------------------------------------------------------------------------------//
        // ----------------------------   Modificado    ---------------------------------------//
        // ------------------------------------------------------------------------------------//

        private void btnLimpiar_Click(object sender, EventArgs e)
        {

            txtCodigo.Text = "";
            txtProducto.Text = "";
            txtCantidad.Text = "0";
            txtPrecio.Text = "0";
            cmbClasificacion.Text = "";
            txtPorIVA.Text = "";
            txtPorDescuento.Text = "0";
            lblTotal.Text = "";
        }

        // ------------------------------------------------------------------------------------//
        // ----------------------------   Modificado    ---------------------------------------//
        // ------------------------------------------------------------------------------------//

        private void lblTotal_Click(object sender, EventArgs e)
        {

        }

        private void dgvProductos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void txtCodigo_TextChanged(object sender, EventArgs e)
        {

        }
    }
    
}
