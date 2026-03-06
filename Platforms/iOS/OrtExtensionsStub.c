// Stub for ONNX Runtime custom ops registration required when statically linking on iOS.
// ORT calls this symbol at session init; returning NULL means no custom ops to register.
typedef struct OrtSessionOptions OrtSessionOptions;
typedef struct OrtApiBase OrtApiBase;
typedef struct OrtStatus OrtStatus;

OrtStatus* RegisterCustomOps(OrtSessionOptions* options, const OrtApiBase* api_base) {
    return (OrtStatus*)0;
}
