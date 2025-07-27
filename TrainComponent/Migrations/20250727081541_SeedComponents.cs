using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainComponent.Migrations
{
    /// <inheritdoc />
    public partial class SeedComponents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
                    INSERT INTO Components (Name, UniqueNumber, CanAssignQuantity) VALUES
                    ('Engine', 'ENG123', 0),
                    ('Passenger Car', 'PAS456', 0),
                    ('Freight Car', 'FRT789', 0),
                    ('Wheel', 'WHL101', 1),
                    ('Seat', 'STS234', 1),
                    ('Window', 'WIN567', 1),
                    ('Door', 'DR123', 1),
                    ('Control Panel', 'CTL987', 1),
                    ('Light', 'LGT456', 1),
                    ('Brake', 'BRK789', 1),
                    ('Bolt', 'BLT321', 1),
                    ('Nut', 'NUT654', 1),
                    ('Engine Hood', 'EH789', 0),
                    ('Axle', 'AX456', 0),
                    ('Piston', 'PST789', 0),
                    ('Handrail', 'HND234', 1),
                    ('Step', 'STP567', 1),
                    ('Roof', 'RF123', 0),
                    ('Air Conditioner', 'AC789', 0),
                    ('Flooring', 'FLR456', 0),
                    ('Mirror', 'MRR789', 1),
                    ('Horn', 'HRN321', 0),
                    ('Coupler', 'CPL654', 0),
                    ('Hinge', 'HNG987', 1),
                    ('Ladder', 'LDR456', 1),
                    ('Paint', 'PNT789', 0),
                    ('Decal', 'DCL321', 1),
                    ('Gauge', 'GGS654', 1),
                    ('Battery', 'BTR987', 0),
                    ('Radiator', 'RDR456', 0);
                "
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Components WHERE Id BETWEEN 1 AND 30;");
        }
    }
}
