namespace Proyecto_cliente
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.TUsername = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Password = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.IniciarSesion = new System.Windows.Forms.Button();
            this.Registro = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TUsername
            // 
            this.TUsername.Location = new System.Drawing.Point(14, 41);
            this.TUsername.Margin = new System.Windows.Forms.Padding(2);
            this.TUsername.Name = "TUsername";
            this.TUsername.Size = new System.Drawing.Size(159, 20);
            this.TUsername.TabIndex = 10;
            this.TUsername.TextChanged += new System.EventHandler(this.TUsername_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 26);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Nombre de usuario";
            // 
            // Password
            // 
            this.Password.Location = new System.Drawing.Point(14, 108);
            this.Password.Margin = new System.Windows.Forms.Padding(2);
            this.Password.Name = "Password";
            this.Password.Size = new System.Drawing.Size(159, 20);
            this.Password.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 93);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Contraseña";
            // 
            // IniciarSesion
            // 
            this.IniciarSesion.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IniciarSesion.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.IniciarSesion.Location = new System.Drawing.Point(14, 170);
            this.IniciarSesion.Margin = new System.Windows.Forms.Padding(2);
            this.IniciarSesion.Name = "IniciarSesion";
            this.IniciarSesion.Size = new System.Drawing.Size(95, 32);
            this.IniciarSesion.TabIndex = 14;
            this.IniciarSesion.Text = "Iniciar sesión";
            this.IniciarSesion.UseVisualStyleBackColor = true;
            this.IniciarSesion.Click += new System.EventHandler(this.IniciarSesion_Click);
            // 
            // Registro
            // 
            this.Registro.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Registro.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Registro.Location = new System.Drawing.Point(136, 170);
            this.Registro.Margin = new System.Windows.Forms.Padding(2);
            this.Registro.Name = "Registro";
            this.Registro.Size = new System.Drawing.Size(95, 32);
            this.Registro.TabIndex = 15;
            this.Registro.Text = "Registrarse";
            this.Registro.UseVisualStyleBackColor = true;
            this.Registro.Click += new System.EventHandler(this.Registro_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(244, 215);
            this.Controls.Add(this.Registro);
            this.Controls.Add(this.IniciarSesion);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Password);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TUsername);
            this.Name = "Form2";
            this.Text = "Form2";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TUsername;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox Password;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button IniciarSesion;
        private System.Windows.Forms.Button Registro;
    }
}