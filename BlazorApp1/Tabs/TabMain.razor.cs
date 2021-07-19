using Microsoft.AspNetCore.Components.Web;
using Syncfusion.Blazor.ProgressBar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Dynamic;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorApp1.Tabs
{
    public partial class TabMain
    {

        #region propiedades de vista

        public PopUp TabPopUp;
        public int SelectedTab { get; set; } = 0;
        private bool VisibleProperty { get; set; } = false;

        #endregion


        #region propiedades de la consulta

        public DateTime? fecini { get; set; } = DateTime.Now;
        public DateTime? fecfin { get; set; } = DateTime.Now;

        public string Ref_ini { get; set; } = "";
        public string Ref_fin { get; set; } = "";

        public string Bod_ini { get; set; } = "";
        public string Bod_fin { get; set; } = "";

        public string Cli_ini { get; set; } = "";
        public string Cli_fin { get; set; } = "";

        public string Ven_ini { get; set; } = "";
        public string Ven_fin { get; set; } = "";

        public string Lin_ini { get; set; } = "";
        public string Lin_fin { get; set; } = "";

        public string Gru_ini { get; set; } = "";
        public string Gru_fin { get; set; } = "";

        #endregion

        #region data sources

        public List<ExpandoObject> DataGridReferencia { get; set; }
        public List<ExpandoObject> DataGridBodega { get; set; }
        public List<ExpandoObject> DataGridCliente { get; set; }
        public List<ExpandoObject> DataGridVendedor { get; set; }
        public List<ExpandoObject> DataGridMarca { get; set; }
        public List<ExpandoObject> DataGridLinea { get; set; }

        #endregion

        [Inject]
        public IConfiguration Configuration { get; set; }

        [Inject]
        public IJSRuntime js { get; set; }

        protected override void OnInitialized()
        {
        }


        private string ArmaWhere()
        {
            try
            {

                string cadenawhere = "";
                string RefI = Ref_ini.Trim();
                string RefF = Ref_fin.Trim();
                string BodI = Bod_ini.Trim();
                string BodF = Bod_fin.Trim();
                string TerI = Cli_ini.Trim();
                string VenI = Cli_fin.Trim();
                string TipI = Lin_ini.Trim();
                string TipF = Lin_fin.Trim();
                string GruI = Gru_ini.Trim();
                string GruF = Gru_fin.Trim();


                //string ImpI = TextBoxImpI.Text.Trim();
                if (!string.IsNullOrEmpty(RefI) && !string.IsNullOrEmpty(RefF))
                {
                    cadenawhere += " and  cue.cod_ref between '" + RefI + "' and '" + RefF + "'";
                }
                if (!string.IsNullOrEmpty(BodI) && !string.IsNullOrEmpty(BodF))
                {
                    cadenawhere += " and  cue.cod_bod between '" + BodI + "' and '" + BodF + "'";
                }
                if (!string.IsNullOrEmpty(TerI))
                {
                    cadenawhere += " and  cab.cod_cli='" + TerI + "'";
                }
                if (!string.IsNullOrEmpty(VenI))
                {
                    cadenawhere += " and  cab.cod_Ven='" + VenI + "'";
                }

                if (!string.IsNullOrEmpty(TipI) && !string.IsNullOrEmpty(TipF))
                {
                    cadenawhere += " and  ref.cod_tip between '" + TipI + "' and '" + TipF + "'";
                }
                if (!string.IsNullOrEmpty(GruI) && !string.IsNullOrEmpty(GruF))
                {
                    cadenawhere += " and  ref.cod_gru between '" + GruI + "' and '" + GruF + "'";
                }

                return cadenawhere;


            }
            catch (Exception w)
            {
                Console.WriteLine("ERRRRROR:" + w);
                return "";
            }

        }

        public void BtnOpenSearch(string tipo)
        {
            try
            {

                string tabla = "";
                string codigo = "";
                string nombre = "";
                string where = "";


                switch (tipo)
                {
                    case "referencia":
                        tabla = "inmae_ref"; codigo = "cod_ref"; nombre = "nom_ref";
                        break;
                    case "bodega":
                        tabla = "inmae_bod"; codigo = "cod_bod"; nombre = "nom_bod";
                        break;
                    case "cliente":
                        tabla = "comae_ter"; codigo = "cod_ter"; nombre = "nom_ter";
                        break;
                    case "vendedor":
                        tabla = "inmae_mer"; codigo = "cod_mer"; nombre = "nom_mer";
                        break;
                    case "linea":
                        tabla = "inmae_tip"; codigo = "cod_tip"; nombre = "nom_tip";
                        break;
                    case "grupo":
                        tabla = "inmae_gru"; codigo = "cod_gru"; nombre = "nom_gru";
                        break;

                }


                TabPopUp.OpenPopUp(tabla, codigo, nombre, where, tipo);
            }
            catch (Exception w)
            {
                Console.WriteLine("error openSearch:" + w);
            }
        }

        public void ClickLoadMessage(string tipo, string codigo)
        {

            switch (tipo)
            {
                case "referencia":
                    Ref_ini = codigo;
                    Ref_fin = codigo;
                    break;
                case "bodega":
                    Bod_ini = codigo;
                    Bod_fin = codigo;
                    break;
                case "cliente":
                    Ref_ini = codigo;
                    Ref_fin = codigo;
                    break;
                case "vendedor":
                    Ven_ini = codigo;
                    Ven_fin = codigo;
                    break;
                case "linea":
                    Lin_ini = codigo;
                    Lin_fin = codigo;
                    break;
                case "grupo":
                    Gru_ini = codigo;
                    Gru_fin = codigo;
                    break;
            }
            StateHasChanged();
        }


        private async void ClickEvent(MouseEventArgs args)
        {
            try
            {


                VisibleProperty = true;

                string where = ArmaWhere();

                
                string fec_ini = Convert.ToDateTime(fecini).ToString("dd/MM/yyyy");
                string fec_fin = Convert.ToDateTime(fecfin).ToString("dd/MM/yyyy");

                await js.InvokeAsync<object>("console.log", "Rfec_ini:" + fec_ini + " Rfec_fin:" + fec_fin);

                //string fec_ini = "10/07/2021";
                //string fec_fin = "13/07/2021";


                var slowTask = Task<JsonVentas>.Factory.StartNew(() => GetAll(fec_ini, fec_fin, where));
                await slowTask;

                if (slowTask.IsCompleted)
                {

                    DataGridReferencia = JsonConvert.DeserializeObject<List<ExpandoObject>>(slowTask.Result.JsonReferencias);
                    DataGridBodega = JsonConvert.DeserializeObject<List<ExpandoObject>>(slowTask.Result.JsonBodega);
                    DataGridCliente = JsonConvert.DeserializeObject<List<ExpandoObject>>(slowTask.Result.JsonCliente);
                    DataGridVendedor = JsonConvert.DeserializeObject<List<ExpandoObject>>(slowTask.Result.JsonVendedor);
                    DataGridMarca = JsonConvert.DeserializeObject<List<ExpandoObject>>(slowTask.Result.JsonMarca);
                    DataGridLinea = JsonConvert.DeserializeObject<List<ExpandoObject>>(slowTask.Result.JsonLinea);

                    VisibleProperty = false;
                    SelectedTab = 1;
                    StateHasChanged();
                }

            }
            catch (Exception e)
            {
                js.InvokeAsync<object>("console.log", "error en los datos:" + e);
                Console.WriteLine("error en el click:" + e);
            }
        }



        public JsonVentas GetAll(string fec_ini, string fec_fin, string where)
        {
            try
            {
                using (SqlConnection cnn = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                {
                    cnn.Open();
                    SqlCommand cmd = new SqlCommand();
                    SqlDataAdapter da = new SqlDataAdapter();
                    DataSet ds = new DataSet();
                    cmd = new SqlCommand("_EmpSpConsultaInAnalisisDeVentas", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FechaIni", fec_ini);
                    cmd.Parameters.AddWithValue("@FechaFin", fec_fin);
                    cmd.Parameters.AddWithValue("@Where", where);
                    cmd.Parameters.AddWithValue("@codemp", "010");
                    da = new SqlDataAdapter(cmd);
                    da.SelectCommand.CommandTimeout = 0;
                    da.Fill(ds);


                    JsonVentas json = new JsonVentas();

                    string referencia = JsonConvert.SerializeObject(ds.Tables[0], Formatting.Indented);
                    json.JsonReferencias = referencia;

                    string bodega = JsonConvert.SerializeObject(ds.Tables[1], Formatting.Indented);
                    json.JsonBodega = bodega;

                    string cliente = JsonConvert.SerializeObject(ds.Tables[2], Formatting.Indented);
                    json.JsonCliente = cliente;

                    string vendedor = JsonConvert.SerializeObject(ds.Tables[3], Formatting.Indented);
                    json.JsonVendedor = vendedor;

                    string marca = JsonConvert.SerializeObject(ds.Tables[4], Formatting.Indented);
                    json.JsonMarca = marca;

                    string linea = JsonConvert.SerializeObject(ds.Tables[5], Formatting.Indented);
                    json.JsonLinea = linea;

                    cnn.Close();
                    return json;
                }
            }
            catch (Exception w)
            {
                js.InvokeAsync<object>("console.log", "error en ClickEvent:" + w);
                return null;
            }

        }

        public class JsonVentas
        {

            public string JsonReferencias { get; set; }
            public string JsonBodega { get; set; }
            public string JsonCliente { get; set; }
            public string JsonVendedor { get; set; }
            public string JsonMarca { get; set; }
            public string JsonLinea { get; set; }

        }











    }
}
