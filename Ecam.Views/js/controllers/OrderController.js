"use strict";
define("OrderController", ["knockout", "komapping", "helper"], function (ko, komapping, helper) {
    return function (url, rnd) {
        var self = this;
        this.template = "/Home/Order";
        this.rows = ko.observableArray([]);

        this.target_percentage = ko.observable(1);
        this.stop_loss_percentage = ko.observable(0.5);

        this.addOrder = function () {
            var row = { "symbol": '', "quantity": '', "price": '', "type": "BS", "target": '', "stop_loss": '' };
            self.rows.push(komapping.fromJS(row));
        }

        this.calculate = ko.computed(function () {
            $.each(self.rows(), function (i, row) {
                var quantity = cFloat(row.quantity());
                var price = cFloat(row.price());
                var amount = cFloat(quantity * price);
                //console.log('amount=', amount);
                var targetPercentage = cFloat(self.target_percentage());
                var stopLossPercentage = cFloat(self.stop_loss_percentage());
                var targetAmount = cFloat(price * targetPercentage) / 100;
                var stopLossAmount = cFloat(price * stopLossPercentage) / 100;
                var target = '';
                var stopLoss = '';
                if (row.type() == "SB") {
                    target = cFloat(price - targetAmount);
                    stopLoss = cFloat(price + stopLossAmount);
                } else {
                    target = cFloat(price + targetAmount);
                    stopLoss = cFloat(price - stopLossAmount)
                }
                if (target <= 0) { target = '' }
                if (stopLoss <= 0) { stopLoss = '' }
                row.target(target);
                row.stop_loss(stopLoss);
            });
        });

        this.init = function () {
            var i;
            for (i = 0; i < 5; i++) {
                self.addOrder();
            }
            unblockUI();
        }

    }
}
);