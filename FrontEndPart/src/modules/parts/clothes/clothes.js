import { LightningElement, track, api } from 'lwc';
const axios = require('axios');

const LOCAL_HOST = 'https://localhost:7186/';

export default class Clothes extends LightningElement {

    @track operation = 'Read';
    @track records = [];

    label = '123';

    selectedIndex;

    async connectedCallback(){
        await this.getRecords();
    }

    get key(){ return Math.random().toString(16).slice(2); }

    async getRecords(){
        let result = await axios.get(LOCAL_HOST + 'Clothes');
        this.records = JSON.parse(JSON.stringify(result)).data;
        this.records.sort((a,b) => a.Id - b.Id);
        console.log(JSON.parse(JSON.stringify(result)).data);
    }

    @api
    handleRowChanged(event){
        let index = event.target.dataset.index;
        let name = event.target.dataset.name;
        let value = event.target.innerHTML;

        this.records[index][name] = value;
        console.log(JSON.parse(JSON.stringify(this.records)));
    }

    handleAddRowClick(){
        this.records.push({Id: undefined ,Name: '', Email: '', Phone: ''});
    }

    async handleSaveClick(){
        let hasError = false;

        if(!hasError){
            await axios.put(LOCAL_HOST + 'Clothes' + '?jsonData=' + JSON.stringify(this.records));
            this.getRecords();
        }
    }

    handleCheckboxClick(event){
        let index = event.target.dataset.index;
        this.selectedIndex = +index;
        let checkboxes = this.template.querySelectorAll('.checkboxCustomer');
        checkboxes.forEach((el, i) => {
            if(i != index){
                el.checked = false;
            }
        });
    }

    async handleDeleteClick(){
        if(this.selectedIndex == null) return;

        if(!this.records[this.selectedIndex].id){
            this.records.splice(this.selectedIndex, 1);
        } else {
            await axios.delete(LOCAL_HOST + 'Clothes' + '?Id=' + this.records[this.selectedIndex].id);
            this.getRecords();
        }
    }

    async handleSellclick(){
        if(this.selectedIndex == null) return;

        if(this.records[this.selectedIndex].quantity == 0){
            this.handleDeleteClick();
        } else {
            this.records[this.selectedIndex].quantity -= 1;
            this.handleSaveClick();
        }
    }

    handleRefreshClick(){
        this.getRecords();
    }

}
