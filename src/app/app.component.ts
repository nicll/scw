import {Component} from '@angular/core';
import { ProductService } from './productservice';
import { Product } from './product';
import {LazyLoadEvent, MenuItem} from 'primeng/api';
import { SelectItem } from 'primeng/api';
import {MessageService} from 'primeng/api';
import {jsPDF} from 'jspdf';
import 'jspdf-autotable';
import {FileUploadModule} from 'primeng/fileupload';
import {HttpClientModule} from '@angular/common/http';
import * as XLSX from 'xlsx';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  providers: [MessageService],
  styles: [`
      :host ::ng-deep .p-cell-editing {
          padding-top: 0 !important;
          padding-bottom: 0 !important;
      }
  `]
})

export class AppComponent {
    products1: Product[];
    cols: any[];
    products2: Product[];

    selectedProduct: Product;

    loading: boolean;

    statuses: SelectItem[];

    clonedProducts: { [s: string]: Product; } = {};

    exportColumns: any[];
    items: MenuItem[];
    willDownload = false;



    constructor(private productService: ProductService, private messageService: MessageService) { }

    ngOnInit() {
        this.loading = true;
        setTimeout(() => {
            this.productService.getProductsSmall().then(data => this.products1 = data);
            this.productService.getProductsSmall().then(data => this.products2 = data);
            this.loading = false;
        }, 1000);
        this.cols = [
            { field: 'Re Order Column', header: 'Re Order Column' },
            { field: 'Code', header: 'Code' },
            { field: 'Name', header: 'Name' },
            { field: 'Category', header: 'Category' },
            { field: 'Quantity', header: 'Quantity' }
        ];

        // this.productService.getProductsSmall().then(data => this.products1 = data);
        // this.productService.getProductsSmall().then(data => this.products2 = data);
        console.log(localStorage.getItem('product'));
        // this.products1 = localStorage.getItem('product');
        // this.products2 = localStorage.getItem('product');


        this.statuses = [{label: 'In Stock', value: 'INSTOCK'}, {label: 'Low Stock', value: 'LOWSTOCK'}, {label: 'Out of Stock', value: 'OUTOFSTOCK'}];

        this.exportColumns = this.statuses.map(col => ({title: col.label, dataKey: col.value}));

        this.items = [
            {label: 'View', icon: 'pi pi-fw pi-search', command: () => this.viewProduct(this.selectedProduct)},
            {label: 'Delete', icon: 'pi pi-fw pi-times', command: () => this.deleteProduct(this.selectedProduct)}
        ];
    }
    viewProduct(product: Product) {
        this.messageService.add({severity: 'info', summary: 'Product Selected', detail: product.name });
    }

    onFileChange(ev) {
        let workBook = null;
        let jsonData = null;
        const reader = new FileReader();
        const file = ev.target.files[0];
        reader.onload = (event) => {
            const data = reader.result;
            workBook = XLSX.read(data, { type: 'binary' });
            jsonData = workBook.SheetNames.reduce((initial, name) => {
                const sheet = workBook.Sheets[name];
                initial[name] = XLSX.utils.sheet_to_json(sheet);
                return initial;
            }, {});
            const dataString = JSON.stringify(jsonData);
            document.getElementById('output').innerHTML = dataString.slice(0, 300).concat('...');
            this.setDownload(dataString);
        };
        reader.readAsBinaryString(file);
    }
    setDownload(data) {
        this.willDownload = true;
        setTimeout(() => {
            const el = document.querySelector('#download');
            el.setAttribute('href', `data:text/json;charset=utf-8,${encodeURIComponent(data)}`);
            el.setAttribute('download', 'xlsxtojson.json');
        }, 1000);
    }



    deleteProduct(product: Product) {
        this.products1 = this.products1.filter((p) => p.id !== product.id);
        this.messageService.add({severity: 'info', summary: 'Product Deleted', detail: product.name});
        this.selectedProduct = null;
    }

    onRowEditInit(product: Product) {
        this.clonedProducts[product.id] = {...product};
    }

    onRowEditSave(product: Product) {
        if (product.price > 0) {
            delete this.clonedProducts[product.id];
            localStorage.setItem('product', JSON.stringify(this.products1));
            console.log(localStorage.getItem('product'));
            this.messageService.add({severity: 'success', summary: 'Success', detail: 'Product is updated'});
        } else {
            this.messageService.add({severity: 'error', summary: 'Error', detail: 'Invalid Price'});
        }
    }

    onRowEditCancel(product: Product, index: number) {
        this.products2[index] = this.clonedProducts[product.id];
        delete this.products2[product.id];
    }

    exportExcel() {
        import('xlsx').then(xlsx => {
            const worksheet = xlsx.utils.json_to_sheet(this.products1);
            const workbook = { Sheets: { 'data': worksheet }, SheetNames: ['data'] };
            const excelBuffer: any = xlsx.write(workbook, { bookType: 'xlsx', type: 'array' });
            this.saveAsExcelFile(excelBuffer, 'products');
        });
    }

    saveAsExcelFile(buffer: any, fileName: string): void {
        import('file-saver').then(FileSaver => {
            const EXCEL_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
            const EXCEL_EXTENSION = '.xlsx';
            const data: Blob = new Blob([buffer], {
                type: EXCEL_TYPE
            });
            FileSaver.saveAs(data, fileName + '_export_' + new Date().getTime() + EXCEL_EXTENSION);
        });
    }

}
