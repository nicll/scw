import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { HandleError, HttpErrorHandler } from '../http-error-handler.service';
import { DataSet } from '../Models/DataSet';

@Injectable({
  providedIn: 'root'
})
export class TableService {
  private readonly handleError: HandleError;
  baseUri: string='http://localhost:5000/api';
  constructor(private http:HttpClient,httpErrorHandler: HttpErrorHandler) {
    this.handleError = httpErrorHandler.createHandleError('HeroesService');
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
  public DeleteSheet(id:string):Observable<DataSet>{
    return this.http.delete<any[]>(this.baseUri+"/data/sheet/"+id,{withCredentials:true}).pipe(
      catchError(err=>{
        this.handleError('GetSheet');
        console.error(err);
        return throwError(err);
      }),
      map(sheet=>{return new DataSet(sheet);})
    );
  }
  public PostDataSet(id:string,table:Array<any>):Observable<Array<any>>{
    return this.http.post<Array<any>>(this.baseUri+"/data/dataset/"+id,table,{withCredentials:true}).pipe(
      catchError(err=>{
        this.handleError('PostDataSet',table);
        console.error(err);
        return throwError(err);
      }),
      map(_=>{return table;})
    );
  }
  public DeleteDataSet(id:string):Observable<DataSet>{
    return this.http.delete<any[]>(this.baseUri+"/data/dataset/"+id,{withCredentials:true}).pipe(
      catchError(err=>{
        this.handleError('GetSheet');
        console.error(err);
        return throwError(err);
      }),
      map(sheet=>{return new DataSet(sheet);})
    );
  }
}
