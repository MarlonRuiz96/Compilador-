<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Compilador.Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <main>
        <div>
            <h2>Listado de TOKENS</h2>
            <asp:FileUpload ID="archivoUpload" runat="server" />
            <br /><br />
            <asp:Button ID="AdjuntarBtn" runat="server" Text="Adjuntar" OnClick="AdjuntarBtn_Click" />
           
            <table>
                <thead>
                    <tr>
                        <th>Tipo</th>
                        <th>Valor</th>
                        <th>Posición</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Literal ID="litTokens" runat="server"></asp:Literal>
                </tbody>
            </table>
            
           
            <table>
                <thead>
                    <tr>
                        <th>Gravedad</th>
                        <th>Descripción</th>
                        <th>Valor</th>
                        <th>Posición</th>
                    </tr>
                </thead>
                <tbody>
                    <h2>Errores Léxicos:</h2>
                    <asp:Literal ID="litErroresLexicos" runat="server"></asp:Literal>
                </tbody>
            </table>

            <h2>Errores Sintácticos</h2>
            <table>
                <thead>
                    <tr>
                        <th>Gravedad</th>
                        <th>Descripción</th>
                        <th>Valor</th>
                        <th>Posición</th>
                    </tr>
                </thead>
                <tbody>
                  
                    <asp:Literal ID="litErroresSintacticos" runat="server"></asp:Literal>
                </tbody>
            </table>

            
            <table>
                <thead>
                    <tr>
                        <th>Tipo</th>
                        <th>Valor</th>
                        <th>Posición</th>
                    </tr>
                </thead>
                <tbody>
                    <h2>Generación de código:</h2>
                    <asp:Literal ID="GeneracionCodigo" runat="server"></asp:Literal>
                </tbody>
            </table>
        </div>
    </main>
</asp:Content>

