﻿using OBD.NET.DataTypes;

namespace OBD.NET.OBDData{

public class OxygenSensor1FuelTrim : AbstractOBDData
{
    #region Properties & Fields

    public Volt Voltage => new(A / 200.0, 0, 1.275);
    public Percent ShortTermFuelTrim => new((B / 1.28) - 100, -100, 99.2);
    public bool IsSensorUsed => B != 0xFF;

    #endregion

    #region Constructors

    public OxygenSensor1FuelTrim()
        : base(0x14, 2)
    { }

    #endregion

    #region Methods

    public override string ToString() => ShortTermFuelTrim.ToString();

    #endregion
}}