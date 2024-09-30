using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TransConnect.Migrations
{
    /// <inheritdoc />
    public partial class InitData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    SocialSecurityNumber = table.Column<string>(type: "longtext", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    LastName = table.Column<string>(type: "longtext", nullable: false),
                    FirstName = table.Column<string>(type: "longtext", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Address = table.Column<string>(type: "longtext", nullable: false),
                    Email = table.Column<string>(type: "longtext", nullable: false),
                    Phone = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    DateOfEntry = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Position = table.Column<string>(type: "longtext", nullable: false),
                    Salary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ManagerId = table.Column<Guid>(type: "char(36)", nullable: true),
                    SocialSecurityNumber = table.Column<string>(type: "longtext", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    LastName = table.Column<string>(type: "longtext", nullable: false),
                    FirstName = table.Column<string>(type: "longtext", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Address = table.Column<string>(type: "longtext", nullable: false),
                    Email = table.Column<string>(type: "longtext", nullable: false),
                    Phone = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Employees_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Points",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Points", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PriceList",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    From = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VehicleType = table.Column<int>(type: "int", nullable: false),
                    PriceType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceList", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Discriminator = table.Column<string>(type: "varchar(21)", maxLength: 21, nullable: false),
                    Seats = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Edges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    StartId = table.Column<Guid>(type: "char(36)", nullable: false),
                    EndId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Distance = table.Column<double>(type: "double", nullable: false),
                    Time = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Edges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Edges_Points_EndId",
                        column: x => x.EndId,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Edges_Points_StartId",
                        column: x => x.StartId,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    ClientId = table.Column<Guid>(type: "char(36)", nullable: false),
                    StartId = table.Column<Guid>(type: "char(36)", nullable: false),
                    EndId = table.Column<Guid>(type: "char(36)", nullable: false),
                    DriverId = table.Column<Guid>(type: "char(36)", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ConfirmationDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    PreparationDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeliveryDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CompletionDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CancellationDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    VehicleId = table.Column<Guid>(type: "char(36)", nullable: true),
                    VehicleType = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Discriminator = table.Column<string>(type: "varchar(21)", maxLength: 21, nullable: false),
                    NumOfPassengers = table.Column<int>(type: "int", nullable: true),
                    Materials = table.Column<string>(type: "longtext", nullable: true),
                    Volume = table.Column<double>(type: "double", nullable: true),
                    Purpose = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_Employees_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Orders_Points_EndId",
                        column: x => x.EndId,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_Points_StartId",
                        column: x => x.StartId,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OrderPaths",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    OrderId = table.Column<Guid>(type: "char(36)", nullable: false),
                    FromId = table.Column<Guid>(type: "char(36)", nullable: false),
                    ToId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Number = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderPaths", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderPaths_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderPaths_Points_FromId",
                        column: x => x.FromId,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderPaths_Points_ToId",
                        column: x => x.ToId,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Address", "DateOfBirth", "DateOfEntry", "Email", "FirstName", "Gender", "LastName", "ManagerId", "Phone", "Position", "Salary", "SocialSecurityNumber" },
                values: new object[] { new Guid("ec9c02a9-edf8-4418-8d77-8163f4332ba8"), "48 Thompson Terrace", new DateTime(1990, 7, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2015, 12, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "pbarkley0@spotify.com", "Pearce", 0, "Dupond", null, "724-760-5107", "Directeur Général", 5000m, "833-68-2338" });

            migrationBuilder.InsertData(
                table: "Points",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("093eaa96-84ec-49a8-b7fb-3177a46fe6ab"), "Marseille " },
                    { new Guid("0adfafd1-eb18-4320-8880-e8c7e0d8bc36"), "Toulouse" },
                    { new Guid("28973532-9b3e-4fc2-85d2-186233d8770f"), "Paris" },
                    { new Guid("46881132-143a-4581-9407-476450172392"), "Toulon" },
                    { new Guid("477524a9-05e3-4726-a73c-47bf65e3a1a7"), "Biarritz" },
                    { new Guid("5a9be339-8eac-4928-9f44-53b0206ca5f4"), "Pau" },
                    { new Guid("5c727375-139c-4506-bc13-1aa5627c7e29"), "Monaco" },
                    { new Guid("7bb88574-a587-43e9-884d-073bac6b515d"), "Nimes" },
                    { new Guid("8945ad8b-f714-4744-9ac8-d767b143ab7b"), "Angers" },
                    { new Guid("8b615bc2-b275-4d2b-95e5-3a1dcd7cd97d"), "Marseilles" },
                    { new Guid("978ffde7-46a9-4a87-b090-a185a1ad0aac"), "Bordeaux" },
                    { new Guid("b12865ba-f620-456b-a589-163a088741a4"), "Montpellier" },
                    { new Guid("c04d0d48-0f1c-4bd8-bfa2-0d0cb0bd66c5"), "Avignon" },
                    { new Guid("e9442fc5-3b3a-4590-a6e4-a3296ea85ff4"), "La Rochelle" },
                    { new Guid("f08bc4dd-d79c-43ab-8c2b-e77c5a198afa"), "Lyon" },
                    { new Guid("f112cec4-ca11-46cb-bd44-31736c8d66cf"), "Marseille" },
                    { new Guid("f249ecbc-b723-45e8-9b4e-4f05316e0e55"), "Rouen" }
                });

            migrationBuilder.InsertData(
                table: "PriceList",
                columns: new[] { "Id", "From", "PriceType", "UnitPrice", "VehicleType" },
                values: new object[,]
                {
                    { new Guid("0853230a-e7d0-40d5-ae1e-477a7f2d4f9e"), 10m, 1, 6m, 1 },
                    { new Guid("1254a815-8e04-42d3-94b4-7c7c06528938"), 0m, 0, 10m, 0 },
                    { new Guid("19842728-6269-4e78-bac0-b8f871172ac5"), 10m, 0, 28m, 4 },
                    { new Guid("1fa8309d-50ea-4abd-9443-b78431b11273"), 0m, 1, 12m, 2 },
                    { new Guid("2572537d-3790-422d-abfc-ac200d52abc0"), 10m, 0, 18m, 3 },
                    { new Guid("28ecb294-28b2-4007-8eb3-68982f2a425a"), 10m, 1, 11m, 2 },
                    { new Guid("2e6e35f7-8bbb-4b00-85bf-5c4ff0ec9255"), 0m, 1, 7m, 1 },
                    { new Guid("3e19502c-a3e4-460b-b549-e58b17ef8c87"), 0m, 1, 15m, 4 },
                    { new Guid("50ffe86d-f8bf-47a2-a169-2f3235d1d6f2"), 0m, 0, 20m, 3 },
                    { new Guid("547d8bc2-bc45-40e6-a8f0-575dfd178163"), 0m, 0, 30m, 4 },
                    { new Guid("64f8515f-7a0c-4ab6-a884-d64cccb77821"), 10m, 0, 8m, 0 },
                    { new Guid("8aaa9608-8722-4a1f-9938-a0a4add22891"), 10m, 1, 14m, 4 },
                    { new Guid("8c63f9af-7e14-41d4-b237-2b9ee2ca0daf"), 0m, 1, 5m, 0 },
                    { new Guid("9ae620dc-4a4a-4822-b381-2de7162e984c"), 0m, 0, 15m, 1 },
                    { new Guid("9c2f3138-a1ca-491c-af61-99eb6fbce77d"), 10m, 1, 9m, 3 },
                    { new Guid("a8dd34ef-59c5-4936-a019-e6240a98a926"), 10m, 0, 12m, 1 },
                    { new Guid("b60cf37d-4f30-4f82-a9be-a91e0fac314f"), 10m, 0, 22m, 2 },
                    { new Guid("bf07345b-59c8-46db-877d-8a40cc989256"), 0m, 1, 10m, 3 },
                    { new Guid("e40e52fc-e586-4ae8-b799-2eaaa3938d26"), 0m, 0, 25m, 2 },
                    { new Guid("fb8529e2-4a94-4e8c-b5c6-a119986a2951"), 10m, 1, 4m, 0 }
                });

            migrationBuilder.InsertData(
                table: "Vehicles",
                columns: new[] { "Id", "Discriminator" },
                values: new object[,]
                {
                    { new Guid("97422a5d-ac6c-43cb-a4a1-f5d082adaf1c"), "RefrigeratedTruck" },
                    { new Guid("9c5f79b6-8614-4879-92a6-24d830a97fb1"), "Van" }
                });

            migrationBuilder.InsertData(
                table: "Vehicles",
                columns: new[] { "Id", "Discriminator", "Seats" },
                values: new object[] { new Guid("bb2d6b61-a9e2-4ef6-bc8b-56236bbe353d"), "Car", 0 });

            migrationBuilder.InsertData(
                table: "Vehicles",
                columns: new[] { "Id", "Discriminator" },
                values: new object[,]
                {
                    { new Guid("ca561fa5-849a-49b7-8409-a36289a5b97d"), "TankerTruck" },
                    { new Guid("d29879c7-3354-4c08-8202-41644d57fcbf"), "DumpTruck" }
                });

            migrationBuilder.InsertData(
                table: "Edges",
                columns: new[] { "Id", "Distance", "EndId", "StartId", "Time" },
                values: new object[,]
                {
                    { new Guid("19871e0b-710b-4ad4-8e1a-5a92806938a9"), 99.0, new Guid("c04d0d48-0f1c-4bd8-bfa2-0d0cb0bd66c5"), new Guid("f112cec4-ca11-46cb-bd44-31736c8d66cf"), 60 },
                    { new Guid("25edb1f3-a573-42b6-98c4-4a86ada89e19"), 464.0, new Guid("f08bc4dd-d79c-43ab-8c2b-e77c5a198afa"), new Guid("28973532-9b3e-4fc2-85d2-186233d8770f"), 295 },
                    { new Guid("437003d3-b6f9-40f8-964a-0151a5a98e02"), 224.0, new Guid("093eaa96-84ec-49a8-b7fb-3177a46fe6ab"), new Guid("5c727375-139c-4506-bc13-1aa5627c7e29"), 123 },
                    { new Guid("4fe6e926-1284-4c55-9d3a-a5191b37efec"), 202.0, new Guid("477524a9-05e3-4726-a73c-47bf65e3a1a7"), new Guid("978ffde7-46a9-4a87-b090-a185a1ad0aac"), 107 },
                    { new Guid("58de1b26-b110-4231-83bc-1a222e69a9fe"), 169.0, new Guid("5c727375-139c-4506-bc13-1aa5627c7e29"), new Guid("46881132-143a-4581-9407-476450172392"), 95 },
                    { new Guid("6b489434-d454-46c9-8a1d-0d3e79617f35"), 289.0, new Guid("7bb88574-a587-43e9-884d-073bac6b515d"), new Guid("0adfafd1-eb18-4320-8880-e8c7e0d8bc36"), 146 },
                    { new Guid("8d558802-916e-4b08-aae2-52dcd0ac46ea"), 52.0, new Guid("7bb88574-a587-43e9-884d-073bac6b515d"), new Guid("b12865ba-f620-456b-a589-163a088741a4"), 35 },
                    { new Guid("8f5494a4-f1c9-4bab-8fa7-24386fed6d91"), 133.0, new Guid("f249ecbc-b723-45e8-9b4e-4f05316e0e55"), new Guid("28973532-9b3e-4fc2-85d2-186233d8770f"), 105 },
                    { new Guid("9a24ba87-211d-4c05-a5eb-a0f4a2a3793e"), 126.0, new Guid("8b615bc2-b275-4d2b-95e5-3a1dcd7cd97d"), new Guid("7bb88574-a587-43e9-884d-073bac6b515d"), 73 },
                    { new Guid("aa61dd64-dc6a-4e0f-90d3-bdf60cb317ac"), 187.0, new Guid("e9442fc5-3b3a-4590-a6e4-a3296ea85ff4"), new Guid("8945ad8b-f714-4744-9ac8-d767b143ab7b"), 140 },
                    { new Guid("cf3fe099-abcd-4b2f-9a24-4a03b51b91b3"), 309.0, new Guid("0adfafd1-eb18-4320-8880-e8c7e0d8bc36"), new Guid("477524a9-05e3-4726-a73c-47bf65e3a1a7"), 159 },
                    { new Guid("d23a4b9b-9d32-474d-a744-eda72cde7c2b"), 183.0, new Guid("978ffde7-46a9-4a87-b090-a185a1ad0aac"), new Guid("e9442fc5-3b3a-4590-a6e4-a3296ea85ff4"), 98 },
                    { new Guid("d7151037-0cfb-4425-af6a-300f22958d05"), 294.0, new Guid("8945ad8b-f714-4744-9ac8-d767b143ab7b"), new Guid("28973532-9b3e-4fc2-85d2-186233d8770f"), 191 },
                    { new Guid("dd97114b-503d-4fc7-82f1-3ac6a40fc2d7"), 193.0, new Guid("0adfafd1-eb18-4320-8880-e8c7e0d8bc36"), new Guid("5a9be339-8eac-4928-9f44-53b0206ca5f4"), 101 }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Address", "DateOfBirth", "DateOfEntry", "Email", "FirstName", "Gender", "LastName", "ManagerId", "Phone", "Position", "Salary", "SocialSecurityNumber" },
                values: new object[,]
                {
                    { new Guid("2d654c99-6fec-480e-b0f8-bc81571e3a91"), "93855 Heath Junction", new DateTime(1974, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2019, 10, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "rwinborn2@hibu.com", "Raeann", 0, "Fertard", new Guid("ec9c02a9-edf8-4418-8d77-8163f4332ba8"), "764-565-2052", "Directeur des opérations", 3480m, "509-39-5602" },
                    { new Guid("7cf247c5-831e-446d-b128-16e309688be4"), "6739 Jenna Place", new DateTime(1983, 1, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2018, 6, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "jbiesinger4@github.com", "Jedediah", 0, "GripSous", new Guid("ec9c02a9-edf8-4418-8d77-8163f4332ba8"), "584-130-3118", "Directeur Financier", 3185m, "830-31-4802" },
                    { new Guid("b056aea5-fae1-4807-91a7-a6b77661b862"), "8 Mitchell Point", new DateTime(1990, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 11, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "skirgan1@is.gd", "Shannen", 1, "Fiesta", new Guid("ec9c02a9-edf8-4418-8d77-8163f4332ba8"), "224-178-9714", "Directrice Commerciale", 3959m, "214-26-3592" },
                    { new Guid("bc08594b-d2f2-4ce0-bbc7-f95188ea26a5"), "65280 Burning Wood Place", new DateTime(1992, 12, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 6, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "mbegley3@facebook.com", "Mildrid", 1, "Joyeuse", new Guid("ec9c02a9-edf8-4418-8d77-8163f4332ba8"), "535-342-0332", "Directrice des RH", 3304m, "809-30-9487" },
                    { new Guid("01107920-0e65-412d-9dca-b67891678f66"), "67726 Mcguire Avenue", new DateTime(1999, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2015, 7, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "gmohammed6@163.com", "Grange", 1, "Fermi", new Guid("b056aea5-fae1-4807-91a7-a6b77661b862"), "794-623-0664", "Commerciale", 2301m, "131-14-8472" },
                    { new Guid("1ba0bb03-ee36-4ce9-81d9-caba15ce1ab2"), "11 Nancy Alley", new DateTime(1972, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2020, 3, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "ajoerninga@is.gd", "Adella", 1, "ToutleMonde", new Guid("bc08594b-d2f2-4ce0-bbc7-f95188ea26a5"), "602-799-3984", "Contrats", 2572m, "419-93-2623" },
                    { new Guid("933144a2-8c54-483e-8a83-908f7f17923e"), "53 Coolidge Plaza", new DateTime(1993, 9, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2018, 5, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "kallikerc@sciencedaily.com", "Katinka", 0, "GrosSous", new Guid("7cf247c5-831e-446d-b128-16e309688be4"), "184-423-4452", "Controleur de Gestion", 2937m, "768-63-7325" },
                    { new Guid("971270ad-cd01-4e86-8525-e8fe1503dbcd"), "4406 Ryan Way", new DateTime(1970, 11, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2016, 8, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "tbuckettb@xrea.com", "Terri-jo", 0, "Picsou", new Guid("7cf247c5-831e-446d-b128-16e309688be4"), "745-927-0863", "Direction comptable", 2973m, "161-46-5816" },
                    { new Guid("bacd7e31-759a-43de-b6ff-2053ee252166"), "0169 Morningstar Road", new DateTime(1976, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2017, 6, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "skinnar5@ted.com", "Sigvard", 0, "Forge", new Guid("b056aea5-fae1-4807-91a7-a6b77661b862"), "756-202-9590", "Commercial", 2467m, "823-45-9649" },
                    { new Guid("cdafcb41-f43c-43b1-9c0f-292cf761c3a4"), "95778 Lakeland Terrace", new DateTime(1977, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2020, 2, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "ghazeltine9@wix.com", "Greta", 1, "Couleur", new Guid("bc08594b-d2f2-4ce0-bbc7-f95188ea26a5"), "948-301-1911", "Formation", 2632m, "761-30-1257" },
                    { new Guid("dc520825-97ef-4b66-a1ef-dd3daa236948"), "81843 Myrtle Pass", new DateTime(1992, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2020, 9, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "omarguerite8@deliciousdays.com", "Osborne", 1, "Prince", new Guid("2d654c99-6fec-480e-b0f8-bc81571e3a91"), "349-391-5249", "Chef d'Equipe", 2757m, "329-48-0811" },
                    { new Guid("df60222f-eb01-4a42-8780-44b6b7850a2f"), "28506 John Wall Junction", new DateTime(1985, 3, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2018, 1, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "pqueen7@bizjournals.com", "Pierre", 0, "Royal", new Guid("2d654c99-6fec-480e-b0f8-bc81571e3a91"), "286-496-4478", "Chef Equipe", 2300m, "361-63-4368" },
                    { new Guid("194a3805-66b1-4dea-a241-9b72877151c4"), "8704 Laurel Crossing", new DateTime(1976, 8, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2015, 12, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "wmcelmurrayd@answers.com", "Woodie", 0, "Romu", new Guid("df60222f-eb01-4a42-8780-44b6b7850a2f"), "225-855-0185", "Chauffeur", 1948m, "394-89-9842" },
                    { new Guid("1d6f4776-f49b-43a3-8275-eb3626568231"), "212 Commercial Terrace", new DateTime(1981, 6, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2020, 11, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "hvausei@dropbox.com", "Hi", 1, "Fournier", new Guid("971270ad-cd01-4e86-8525-e8fe1503dbcd"), "266-230-2187", "Comptable", 1525m, "275-33-7461" },
                    { new Guid("54413841-b0ff-4ee0-9a35-ffc2013b2da7"), "1065 American Ash Street", new DateTime(1989, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 11, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "lzorere@reddit.com", "Leona", 1, "Romi", new Guid("df60222f-eb01-4a42-8780-44b6b7850a2f"), "473-618-6575", "Chauffeur", 1846m, "843-45-5234" },
                    { new Guid("54548b27-43f6-4fa9-a641-7e6bf466f7f0"), "52760 Crownhardt Terrace", new DateTime(1989, 8, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "prichesg@sun.com", "Paco", 1, "Rome", new Guid("dc520825-97ef-4b66-a1ef-dd3daa236948"), "603-772-5979", "Chauffeur", 1479m, "727-91-3168" },
                    { new Guid("58f34492-568b-418b-badd-597fe0a96e96"), "7199 Hollow Ridge Crossing", new DateTime(1981, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2016, 2, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "asolmanj@youku.com", "Averill", 1, "Gautier", new Guid("971270ad-cd01-4e86-8525-e8fe1503dbcd"), "963-779-2340", "Comptable", 1799m, "512-15-9115" },
                    { new Guid("652b3adb-e1d0-4fa9-92af-b5cce4ad69da"), "6341 Butterfield Road", new DateTime(1971, 7, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2015, 8, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "nknightlyf@ifeng.com", "Neely", 0, "Roma", new Guid("df60222f-eb01-4a42-8780-44b6b7850a2f"), "572-533-9947", "Chauffeur", 1780m, "307-20-9986" },
                    { new Guid("c4e4500a-b27a-424f-833f-e9f10522a85b"), "87435 Blackbird Road", new DateTime(1989, 12, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2017, 3, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "dsnasdellh@symantec.com", "Dilan", 0, "Rimou", new Guid("dc520825-97ef-4b66-a1ef-dd3daa236948"), "669-272-5558", "Chauffeur", 1786m, "703-46-8885" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Edges_EndId",
                table: "Edges",
                column: "EndId");

            migrationBuilder.CreateIndex(
                name: "IX_Edges_StartId",
                table: "Edges",
                column: "StartId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ManagerId",
                table: "Employees",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPaths_FromId",
                table: "OrderPaths",
                column: "FromId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPaths_OrderId",
                table: "OrderPaths",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPaths_ToId",
                table: "OrderPaths",
                column: "ToId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ClientId",
                table: "Orders",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DriverId",
                table: "Orders",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_EndId",
                table: "Orders",
                column: "EndId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_StartId",
                table: "Orders",
                column: "StartId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_VehicleId",
                table: "Orders",
                column: "VehicleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Edges");

            migrationBuilder.DropTable(
                name: "OrderPaths");

            migrationBuilder.DropTable(
                name: "PriceList");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Points");

            migrationBuilder.DropTable(
                name: "Vehicles");
        }
    }
}
