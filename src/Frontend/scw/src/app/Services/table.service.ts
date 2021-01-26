import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Data } from '@angular/router';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { HandleError, HttpErrorHandler } from '../http-error-handler.service';
import { DataSet } from '../Models/DataSet';

@Injectable({
  providedIn: 'root'
})
export class TableService {
  private handleError: HandleError;
  baseUri: string='http://localhost:5000/api';

  constructor(private http:HttpClient,httpErrorHandler: HttpErrorHandler) {
    this.handleError = httpErrorHandler.createHandleError('HeroesService');
  }
  public GetSheet(id:string):Observable<DataSet>{
    return this.http.get<DataSet>(this.baseUri+"/data/sheet/"+id,{withCredentials:true}).pipe(
      catchError(err=>{
        this.handleError('GetSheet');
        console.error(err);
        return throwError(err);
      }),
      map(sheet=>{return sheet;})
    );
  }
  public PostSheet(id:string,table:DataSet):Observable<DataSet>{
    return this.http.post<DataSet>(this.baseUri+"/data/sheet/"+id,table,{withCredentials:true}).pipe(
      catchError(err=>{
        this.handleError('PostSheet',table);
        console.error(err);
        return throwError(err);
      }),
      map(_=>{return table;})
    );
  }
  public GetDataSet(id:string):Observable<DataSet>{
    return this.http.get<any>(this.baseUri+"/data/dataset/"+id,{withCredentials:true}).pipe(
      catchError(err=>{
        this.handleError('GetSheet');
        console.error(err);
        return throwError(err);
      }),
      map(sheet=>{return sheet;})
    );
  }
  
}
