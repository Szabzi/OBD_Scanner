﻿using OBD.NET.DataTypes;

namespace OBD.NET.OBDData{

public class ShortTermSecondaryOxygenSensorTrimBank13 : AbstractOBDData
{
    #region Properties & Fields

    public Percent Bank1 => new((A / 1.28) - 100, -100, 99.2);
    public Percent Bank3 => new((B / 1.28) - 100, -100, 99.2);

    #endregion

    #region Constructors

    public ShortTermSecondaryOxygenSensorTrimBank13()
        : base(0x55, 2)
    { }

    #endregion

    #region Methods

    public override string ToString() => Bank1.ToString();

    #endregion
}}