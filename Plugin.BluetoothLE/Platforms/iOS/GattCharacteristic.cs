using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using CoreBluetooth;
using Foundation;


namespace Plugin.BluetoothLE
{
    public partial class GattCharacteristic : AbstractGattCharacteristic
    {
        Queue<byte[]> toWrite { get; } = new Queue<byte[]>();

        public override void Init()
        {
            this.Peripheral.IsReadyToSendWriteWithoutResponse += Peripheral_IsReadyToSendWriteWithoutResponse;
        }

        private void Peripheral_IsReadyToSendWriteWithoutResponse(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public override IObservable<CharacteristicGattResult> WriteWithoutResponse(byte[] value)
        {
            toWrite.Enqueue(value);
            return Observable.Return(new CharacteristicGattResult(this, value));
        }

        private IObservable<CharacteristicGattResult> Send(byte[] value)
        {
            var data = NSData.FromArray(value);

            var typeOfWrite = this.Peripheral.CanSendWriteWithoutResponse ?
                CBCharacteristicWriteType.WithoutResponse : CBCharacteristicWriteType.WithResponse;

            this.Peripheral.WriteValue(data, this.NativeCharacteristic, typeOfWrite);
            
            return Observable.Return(new CharacteristicGattResult(this, value));
        }

        //public override IObservable<CharacteristicGattResult> WriteWithoutResponse(byte[] value)
        //{
        //    if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
        //        return this.NewInternalWrite(value);

        //    return Observable.Return(this.InternalWrite(value));
        //}


        //IObservable<CharacteristicGattResult> NewInternalWrite(byte[] value) => Observable.Create<CharacteristicGattResult>(ob =>
        //{
        //    EventHandler handler = null;
        //    if (this.Peripheral.CanSendWriteWithoutResponse)
        //    {
        //        ob.Respond(this.InternalWrite(value));
        //    }
        //    else
        //    {
        //        handler = new EventHandler((sender, args) => ob.Respond(this.InternalWrite(value)));
        //        this.Peripheral.IsReadyToSendWriteWithoutResponse += handler;
        //    }
        //    return () =>
        //    {
        //        if (handler != null)
        //            this.Peripheral.IsReadyToSendWriteWithoutResponse -= handler;
        //    };
        //});


        //CharacteristicGattResult InternalWrite(byte[] value)
        //{
        //    var data = NSData.FromArray(value);
        //    this.Peripheral.WriteValue(data, this.NativeCharacteristic, CBCharacteristicWriteType.WithoutResponse);
        //    return new CharacteristicGattResult(this, value);
        //}
    }
}