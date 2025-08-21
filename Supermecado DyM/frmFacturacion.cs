using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Supermecado_DyM
{
    public partial class frmFacturacion : Form
    {
        public frmFacturacion()
        {
            InitializeComponent();
        }

        private void txtCodigo_TextChanged(object sender, EventArgs e)
        {
            Consultar_Producto();
        }

        private void Consultar_Producto()
        {
            try
            {
                Conexion Conn = new Conexion();
                Conn.LeerCadena();

                SqlCommand ComandoConsultaCliente = new SqlCommand("SP_CONSULTA_PRODUCTO", Conn.LeerCadena());
                DataTable tabla = new DataTable();

                ComandoConsultaCliente.CommandType = CommandType.StoredProcedure;
                ComandoConsultaCliente.Parameters.AddWithValue("@CODIGOPRODUCTO", txtCodigo.Text.Trim());

                SqlDataReader ConsultaCliente = ComandoConsultaCliente.ExecuteReader();


                if (ConsultaCliente.Read())
                {

                    // ------------------------------------------------------------------------------------//
                    // ----------------------------          Modificado         ---------------------------//
                    // ------------------------------------------------------------------------------------//

                    // Llena los datos en los textfields para insertar linea a factura

                    decimal IVA;
                    IVA = Convert.ToDecimal(ConsultaCliente["IVA"].ToString());
                    decimal IVA_ENTERO_PORC = IVA * 100;

                    decimal Descuento;
                    Descuento = Convert.ToDecimal(ConsultaCliente["Descuento"].ToString());
                    decimal DESCUENTO_ENTERO_PORC = Descuento * 100;


                    decimal cantidad_ = Convert.ToDecimal(ConsultaCliente["CANTIDAD"]);
                    decimal precio_ = Convert.ToDecimal(ConsultaCliente["PRECIO"]);
                    decimal iva_ = Convert.ToDecimal(ConsultaCliente["IVA"]);
                    decimal descuento_ = Convert.ToDecimal(ConsultaCliente["DESCUENTO"]);
                    decimal total_Linea_ = 0;

                    // llena todo los campos del query de la dB

                    //  decimal MONTO_IVA = cantidad_ * precio_ * iva_;
                    //  decimal MONTO_DESCUENTO = cantidad_ * precio_ * descuento_;
                    //  decimal TOTAL_LINEA = (cantidad_ * precio_ + MONTO_IVA - MONTO_DESCUENTO);

                    decimal MONTO_IVA2 = 0;
                    decimal Cantidad2 = 0;
                    decimal MONTO_DESCUENTO2 = 0;
                    decimal TOTAL_LINEA2 = 0;

                    txtProducto.Text = ConsultaCliente["PRODUCTO"].ToString();
                    txtPrecio.Text = ConsultaCliente["PRECIO"].ToString();
                    cmbClasificacion.Text = ConsultaCliente["CLASIFICACION"].ToString();
                    txtCantidad.Text = Cantidad2.ToString(); ;
                    txtPorcIVA.Text = IVA_ENTERO_PORC.ToString("0.##");
                    txtPorcDescuento.Text = DESCUENTO_ENTERO_PORC.ToString("0.##");
                    txtMontoIVA.Text = MONTO_IVA2.ToString("0.##");
                    txtMontoDescuento.Text = MONTO_DESCUENTO2.ToString("0.##");
                    txtTotalPorLinea.Text = TOTAL_LINEA2.ToString();

                    Conn.LeerCadena();

                }

                else
                {
                    // Limpiar_Campos();
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Mensaje Importante", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            }
        }



        // ------------------------------------------------------------------------------------//
        // ----------------------------          Modificado         ---------------------------//
        // ------------------------------------------------------------------------------------//



        private void frmFacturacion_Load(object sender, EventArgs e)
        {
            //  LLenar_DataGrid_Productos;
            try
            {
                Conexion Conn = new Conexion();
                SqlCommand cmd = new SqlCommand("TRUNCATE TABLE FACTURACION", Conn.LeerCadena());
                cmd.ExecuteNonQuery();
                Conn.LeerCadena().Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al limpiar la tabla: " + ex.Message);
            }
        }


        // ------------------------------------------------------------------------------------//
        // --------------     Modificado Ingresar  Productos a Factura  -----------------------//
        // ------------------------------------------------------------------------------------//


        private void INGRESAR_FACTURA()
        {
            if (string.IsNullOrWhiteSpace(txtCantidad.Text) || txtCantidad.Text == "0")
            {
                MessageBox.Show("Por favor ingrese un valor en el espacio de cantidad.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Conexion Conn = new Conexion();
                SqlConnection connection = Conn.LeerCadena();

                // Ejecutamos el SP para insertar la línea en la tabla FACTURACION
                SqlCommand ComandoProductos = new SqlCommand("SP_INGRESAR_FACTURA", connection);
                ComandoProductos.CommandType = CommandType.StoredProcedure;

                ComandoProductos.Parameters.AddWithValue("@CODIGO_PRODUCTO", txtCodigo.Text.Trim());
                ComandoProductos.Parameters.AddWithValue("@PRODUCTO", txtProducto.Text.Trim());
                ComandoProductos.Parameters.AddWithValue("@PRECIO", Convert.ToDecimal(txtPrecio.Text.Trim()));
                ComandoProductos.Parameters.AddWithValue("@CANTIDAD", Convert.ToInt32(txtCantidad.Text.Trim()));
                ComandoProductos.Parameters.AddWithValue("@PORCENTAJEIVA", Convert.ToDecimal(txtPorcIVA.Text.Trim()));
                ComandoProductos.Parameters.AddWithValue("@PORCENTAJEDESCUENTO", Convert.ToDecimal(txtPorcDescuento.Text.Trim()));
                //ComandoProductos.Parameters.AddWithValue("@TOTALLINEA", Convert.ToDecimal(txtTotalPorLinea.Text.Trim()));
                decimal precio = Convert.ToDecimal(txtPrecio.Text.Trim());
                int cantidad = Convert.ToInt32(txtCantidad.Text.Trim());
                decimal totalLinea = precio * cantidad;

                ComandoProductos.Parameters.AddWithValue("@TOTALLINEA", totalLinea);
                ComandoProductos.Parameters.AddWithValue("@CLASIFICACION", cmbClasificacion.Text.Trim());
                ComandoProductos.Parameters.AddWithValue("@MONTOIVA", Convert.ToDecimal(txtMontoIVA.Text.Trim()));
                ComandoProductos.Parameters.AddWithValue("@MONTODESCUENTO", Convert.ToDecimal(txtMontoDescuento.Text.Trim()));

                ComandoProductos.ExecuteNonQuery();

                // Refrescamos el DataGrid con TODOS los productos
                LLenar_DataGrid_detalle_factura();


                // Actualizamos totales
                CalcularTotalesFactura();

                MessageBox.Show("Producto ingresado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                connection.Dispose();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // Método para calcular totales de la factura
        private void CalcularTotalesFactura()
        {
            decimal subtotal = 0;
            decimal totalIVA = 0;
            decimal totalDescuento = 0;
            decimal totalFinal = 0;

            foreach (DataGridViewRow row in grdFactura.Rows)
            {
                if (!row.IsNewRow)
                {
                    subtotal += Convert.ToDecimal(row.Cells["TOTALLINEA"].Value);
                    totalIVA += Convert.ToDecimal(row.Cells["MONTOIVA"].Value);
                    totalDescuento += Convert.ToDecimal(row.Cells["MONTODESCUENTO"].Value);
                }
            }

            totalFinal = subtotal + totalIVA - totalDescuento;

            // Asumimos que tienes TextBox o Label para mostrar estos totales
            lblSubtotal.Text = subtotal.ToString("0.00");
            lblIVA.Text = totalIVA.ToString("0.00");
            lblDescuento.Text = totalDescuento.ToString("0.00");
            lblTotalaPagar.Text = totalFinal.ToString("0.00");
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {

            // llamamos a insertar factura

            INGRESAR_FACTURA();
        }



        private void LLenar_DataGrid_detalle_factura()
        {
            try
            {
                Conexion Conn = new Conexion();
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM FACTURACION", Conn.LeerCadena());
                DataTable dt = new DataTable();
                da.Fill(dt);

                grdFactura.DataSource = dt;  // <- cambia miDataGridView por el nombre real de tu grid

                Conn.LeerCadena().Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los productos: " + ex.Message);
            }
        }

        private void cmbFormadePago_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtPago_TextChanged(object sender, EventArgs e)
        {
            Consultar_vuelto();
        }

        private void Consultar_vuelto()
        {
            try
            {
                // Si txtPago está vacío, no hacer nada
                if (string.IsNullOrWhiteSpace(txtPago.Text))
                {
                    lblVuelto.Text = ""; // opcional, limpiar el label
                    return;
                }

                decimal totalFinal = 0;

                foreach (DataGridViewRow row in grdFactura.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        decimal linea = Convert.ToDecimal(row.Cells["TOTALLINEA"].Value);
                        decimal iva = Convert.ToDecimal(row.Cells["MONTOIVA"].Value);
                        decimal descuento = Convert.ToDecimal(row.Cells["MONTODESCUENTO"].Value);

                        totalFinal += (linea + iva - descuento);
                    }
                }

                // Leemos lo que paga el cliente
                decimal montoCliente = 0;
                if (!decimal.TryParse(txtPago.Text.Trim(), out montoCliente))
                {
                    MessageBox.Show("Digite un monto válido para el cliente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                decimal vuelto = montoCliente - totalFinal;
                lblVuelto.Text = "₡" + vuelto.ToString("0.00");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error calculando vuelto: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void txtCantidad_TextChanged(object sender, EventArgs e)
        {
            try
            {
                decimal cantidad = Convert.ToDecimal(txtCantidad.Text);
                decimal precio = Convert.ToDecimal(txtPrecio.Text);
                decimal iva = Convert.ToDecimal(txtPorcIVA.Text) / 100;
                decimal descuento = Convert.ToDecimal(txtPorcDescuento.Text) / 100;

                decimal montoIVA = cantidad * precio * iva;
                decimal montoDescuento = cantidad * precio * descuento;
                decimal totalLinea = (cantidad * precio + montoIVA - montoDescuento);

                txtMontoIVA.Text = montoIVA.ToString("0.##");
                txtMontoDescuento.Text = montoDescuento.ToString("0.##");
                txtTotalPorLinea.Text = totalLinea.ToString("0.##");
            }
            catch
            {
                txtMontoIVA.Text = "0";
                txtMontoDescuento.Text = "0";
                txtTotalPorLinea.Text = "0";
            }
        }

        private void actualizar_motos()
        {


            // decimal cantidad = Convert.ToDecimal(txtCantidad.Text);
            // decimal monto_iva = Convert.ToDecimal(txtMontoIVA.Text);
            //  decimal monto_descuento = Convert.ToDecimal(txtMontoDescuento.Text);



        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void lblSubtotal_Click(object sender, EventArgs e)
        {

        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtCodigo.Text = "";
            txtProducto.Text = "";
            txtPrecio.Text = "";
            cmbClasificacion.Text = "";
            txtCantidad.Text = "";
            txtPorcIVA.Text = "";
            txtPorcDescuento.Text = "";
            txtMontoIVA.Text = "";
            txtMontoDescuento.Text = "";
            txtTotalPorLinea.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void grdFactura_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnPagar_Click(object sender, EventArgs e)
        {
            try
            {
                if (grdFactura.Rows.Count == 0)
                {
                    MessageBox.Show("No hay productos para facturar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string metodoPago = cmbFormadePago.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(metodoPago))
                {
                    MessageBox.Show("Debe seleccionar un método de pago.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                decimal subtotal = 0;
                decimal totalIVA = 0;
                decimal totalDescuento = 0;

                foreach (DataGridViewRow row in grdFactura.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        subtotal += Convert.ToDecimal(row.Cells["TOTALLINEA"].Value);
                        totalIVA += Convert.ToDecimal(row.Cells["MONTOIVA"].Value);
                        totalDescuento += Convert.ToDecimal(row.Cells["MONTODESCUENTO"].Value);
                    }
                }

                decimal totalFinal = subtotal + totalIVA - totalDescuento;
                decimal montoCliente = 0;

                if (metodoPago == "EFECTIVO")
                {
                    if (string.IsNullOrWhiteSpace(txtPago.Text))
                    {
                        MessageBox.Show("Debe ingresar el monto con el que se va a cancelar.",
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (!decimal.TryParse(txtPago.Text.Trim(), out montoCliente))
                    {
                        MessageBox.Show("Debe ingresar un monto válido para el cliente.",
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (montoCliente < totalFinal)
                    {
                        MessageBox.Show("El monto del cliente es insuficiente.",
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    lblVuelto.Text = (montoCliente - totalFinal).ToString("0.##");
                }
                // Guardar la factura donde el usuario elija
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Archivos de texto (*.txt)|*.txt";
                    saveFileDialog.Title = "Guardar factura como";
                    saveFileDialog.FileName = "Factura.txt";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string rutaArchivo = saveFileDialog.FileName;

                        using (StreamWriter sw = new StreamWriter(rutaArchivo, false))
                        {
                            sw.WriteLine("FACTURA DE COMPRA");
                            sw.WriteLine("-----------------");
                            foreach (DataGridViewRow row in grdFactura.Rows)
                            {
                                if (!row.IsNewRow)
                                {
                                    sw.WriteLine($"{row.Cells["PRODUCTO"].Value} | Cant: {row.Cells["CANTIDAD"].Value} | Precio: {row.Cells["PRECIO"].Value} | Total: {row.Cells["TOTALLINEA"].Value}");
                                }
                            }
                            sw.WriteLine("-----------------");
                            sw.WriteLine($"Subtotal: {subtotal:0.##}");
                            sw.WriteLine($"IVA: {totalIVA:0.##}");
                            sw.WriteLine($"Descuento: {totalDescuento:0.##}");
                            sw.WriteLine($"Total a pagar: {totalFinal:0.##}");
                            sw.WriteLine($"Método de pago: {metodoPago}");
                            if (metodoPago == "Efectivo")
                                sw.WriteLine($"Monto entregado: {montoCliente:0.##}");
                            sw.WriteLine($"Vuelto: {montoCliente - totalFinal:0.##}");
                        }

                        MessageBox.Show("Factura generada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Guardado cancelado. La factura no se ha generado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Limpiar tabla temporal
                LimpiarFacturaTemporal();

                // Limpiar DataGridView y campos
                grdFactura.DataSource = null;
                grdFactura.Rows.Clear();

                txtCodigo.Text = "";
                txtProducto.Text = "";
                txtPrecio.Text = "";
                cmbClasificacion.Text = "";
                txtCantidad.Text = "";
                txtPorcIVA.Text = "";
                txtPorcDescuento.Text = "";
                txtMontoIVA.Text = "";
                txtMontoDescuento.Text = "";
                txtTotalPorLinea.Text = "";
                txtPago.Text = "";
                lblVuelto.Text = "0.00";
                lblSubtotal.Text = "0.00";
                lblIVA.Text = "0.00";
                lblDescuento.Text = "0.00";
                lblTotalaPagar.Text = "0.00";

                // Opcional: resetear selección de forma de pago
                cmbFormadePago.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al procesar el pago: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LimpiarFacturaTemporal()
        {
            try
            {
                Conexion Conn = new Conexion();
                SqlConnection connection = Conn.LeerCadena();

                SqlCommand cmd = new SqlCommand("DELETE FROM FACTURACION", connection);
                cmd.ExecuteNonQuery();

                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al limpiar la factura temporal: " + ex.Message);
            }
        }
    }
}
